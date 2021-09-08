// A container is just a collection of other blocks,
export enum BlockType {
    Container,
    Text
}

export enum EditorEventType {
    TextChange,
    BlockOrderChange,
    BlockSelection
}

export interface EditorEvent {
    type: EditorEventType;
    textChange?: {
	
    };
    blockOrderChange?: {

    };
    blockSelection?: {
	selected: Block;
    }
}

export interface TransferBlock {
    type: BlockType;
    tagType: string;
    properties: { [key: string]: string };
    blocks?: TransferBlock[];
    text?: string;
}

export interface Block {
    root: Editor;
    parent: Block;
    type: BlockType;
    tagType: string;
    blocks?: Block[];
    element: HTMLElement;
    handlers: { [key: string]: ((event: EditorEvent) => void)[]; };
};

export interface Editor extends Block {
    selectedBlock: Block;
}

function insertChildBlock(parent: Block, block: Block, insertAt: number) {
    if (parent.type === BlockType.Container) {
        if (parent.blocks.length > insertAt) {
            parent.element.insertBefore(block.element, parent.blocks[insertAt].element);
            parent.blocks.splice(insertAt, 0, block);
        } else {
            parent.element.appendChild(block.element);
            parent.blocks.push(block);
        }
    } else {
        throw "parent block has to be a container";
    }
}

export function moveBlock(block: Block, toBlock: Block, insertAt: number) {
    var blockIndex = block.parent.blocks.indexOf(block);
    block.parent.blocks.splice(blockIndex, 1);
    insertChildBlock(toBlock, block, insertAt);
    emit(toBlock, "block-order", {
	type: EditorEventType.BlockOrderChange,
	blockOrderChange: {}
    });
}

export function subscribe(block: Block, name: string, handler: (event: EditorEvent) => void) {
    if (block.handlers[name] === undefined) {
	block.handlers[name] = [];
    }
    if (block.handlers[name].indexOf(handler) !== -1) {
	block.handlers[name].push(handler);
    }
}

export function unsubscribe(block: Block, name: string, handler: (event: EditorEvent) => void) {
    if (block.handlers[name] !== undefined) {
	var index = block.handlers[name].indexOf(handler);
	if (index !== -1) {
	    block.handlers[name].splice(index, 1);
	}
    }
}

function emit(block: Block, name: string, event: EditorEvent) {
    if (block.root !== block) {
        if (block.root.handlers[name] !== undefined) {
	    var handlers = block.root.handlers[name];
	    for (var i = 0; i < handlers.length; i++) {
		handlers[i](event);
	    }
	}
    }
    if (block.handlers[name] !== undefined) {
	var handlers = block.handlers[name];
	for (var i = 0; i < handlers.length; i++) {
	    handlers[i](event);
	}
    }
}

export function loadContent(block: Block, content: TransferBlock[]) {
    if (block.type !== BlockType.Container) {
	throw "Can't load content into text block";
    }
    // Work from behind so index wont change
    for (let i = block.blocks.length - 1; i >= 0; i--) {
	deleteBlock(block.blocks[i]);
    }
    for (let i = 0; i < content.length; i++) {
	if (content[i].type === BlockType.Text) {
	    let child = makeBlock(block, i, BlockType.Text, content[i].tagType);
	    child.element.innerHTML = content[i].text;
	} else {
	    let child = makeBlock(block, i, BlockType.Container, content[i].tagType);
	    loadContent(child, content[i].blocks);
	}
    }
}

export function getContent(block: Block): TransferBlock[] {
    if (block.type !== BlockType.Container) {
	throw "Can't get content from text block";
    }

    let content: TransferBlock[] = [];
    for (let i = 0; i < block.blocks.length; i++) {
	if (block.blocks[i].type === BlockType.Text) {
	    content.push({
		type: block.blocks[i].type,
		tagType: block.blocks[i].tagType,
		properties: {},
		text: block.blocks[i].element.innerHTML
	    });
	} else {
	    content.push({
		type: block.blocks[i].type,
		tagType: block.blocks[i].tagType,
		properties: {},
		blocks: getContent(block.blocks[i])
	    });
	}
    }
    return content;
}

let eventHandlers = new WeakMap();
export function makeBlock(parent: Block, insertAt: number, type: BlockType, tagType: string): Block {
    function textChange() {
	emit(block, "text-change", {
	    type: EditorEventType.TextChange,
	    textChange: {}
	});
    }
    function blockFocus() {
	block.root.selectedBlock = block;
	emit(block, "block-selection", {
	    type: EditorEventType.BlockSelection,
	    blockSelection: {
		selected: block
	    }
	});
    }
    var element = document.createElement(tagType);
    var block: Block = {
        root: parent.root,
        parent: parent,
        type: type,
        tagType: tagType,
        element: element,
        handlers: {}
    };
    element.addEventListener("focus", blockFocus);
    element.style.userSelect = "contain";
    if (type === BlockType.Text) {
        element.contentEditable = "true";
        element.addEventListener("input", textChange);
    } else if (type === BlockType.Container) {
        block.blocks = [];
    } else {
        throw "Invalid block type";
    }
    insertChildBlock(parent, block, insertAt);
    emit(parent, "block-order", {
	type: EditorEventType.BlockOrderChange,
	blockOrderChange: {}
    });
    eventHandlers.set(block, {
	textChange: textChange,
	blockFocus: blockFocus
    });
    return block;
}

export function deleteBlock(block: Block) {
    if (block.type === BlockType.Container) {
	// Work from behind so index wont change
	for (let i = block.blocks.length - 1; i >= 0; i--) {
	    deleteBlock(block.blocks[i]);
	}
    }
    const handlers = eventHandlers.get(block);
    block.element.removeEventListener("input", handlers.textChange);
    block.element.removeEventListener("focus", handlers.blockFocus);

    let blockIndex = block.parent.blocks.indexOf(block);
    block.parent.blocks.splice(blockIndex, 1);
    block.parent.element.removeChild(block.element);
    
    const parent = block.parent;
    block.root = null;
    block.parent = null
    
    emit(parent, "block-order", {
	type: EditorEventType.BlockOrderChange,
	blockOrderChange: {}
    });
}

export function makeEditor(element: HTMLElement): Editor {
    let block: Editor = {
	selectedBlock: null,
        root: null,
        parent: null,
        type: BlockType.Container,
        tagType: element.tagName,
        element: element,
        blocks: [],
        handlers: {}
    };
    block.root = block;
    block.selectedBlock = null;

    return block;
}

function getSon(parent: Node, grandSon: Node): Node {
    while (grandSon.parentNode !== null) {
	if (grandSon.parentNode === parent) {
	    return grandSon;
	}
	grandSon = grandSon.parentNode
    }
    throw "Not a descendant"
}

interface rangeEntry {
    count: number;
    node: Node;
};

function getCronStart(range: Range): [rangeEntry, rangeEntry] {
    if (range.startContainer === range.endContainer) {
	if (range.startOffset < range.endOffset) {
	    return [{
		count: range.startOffset,
		node: range.startContainer
	    },
		    {
		count: range.endOffset,
		node: range.endContainer
	    }];
	} else {
	    return [{
		count: range.endOffset,
		node: range.endContainer
	    },
		    {
		count: range.startOffset,
		node: range.startContainer
	    }];
	}
    }
    const cronnologi: rangeEntry[] = [];
    const start = getSon(range.commonAncestorContainer, range.startContainer);
    const end = getSon(range.commonAncestorContainer, range.endContainer);
    const parent = range.commonAncestorContainer;
    for (let i = 0; i < parent.childNodes.length; i++) {
	if (parent.childNodes[i] === start) {
	    cronnologi.push({
		count: range.startOffset,
		node: range.startContainer
	    });
	}
	// no else as they can be the same node
	if (parent.childNodes[i] === end) {
	    cronnologi.push({
		count: range.endOffset,
		node: range.endContainer
	    });
	}
    }
    return cronnologi as [rangeEntry, rangeEntry];
}

function makeRangeBold(start: Node, end: Node) {
    if (start === null) {
	return;
    }
    const bold = document.createElement("b");
    const parent = start.parentNode;
    let nextNode = start;
    while (nextNode !== end) {
	const node = nextNode;
	nextNode = nextNode.nextSibling;
	bold.appendChild(node);
    }
    parent.insertBefore(bold, end);
}
export function makeSelectionBlod() {
    var selection = getSelection();

    for(var i = 0; i < selection.rangeCount; i++) {
	const range = selection.getRangeAt(i);

	const selectArea = getCronStart(range);
	if (range.startContainer.parentNode == range.endContainer.parentNode) {
	    let startPoint: Node;
	    let insertPoint: Node;
	    if (selectArea[1].node.TEXT_NODE === 3) {
		const textNode = selectArea[1].node as Text;
		insertPoint = textNode.splitText(selectArea[1].count);
	    } else if (range.startContainer.ELEMENT_NODE === 1) {
		insertPoint = selectArea[1].node.nextSibling;
	    } else {
		throw "unsupported node type";
	    }
	    if (selectArea[0].node.TEXT_NODE === 3) {
		const textNode = selectArea[0].node as Text;
		startPoint = textNode.splitText(selectArea[0].count);
	    } else if (range.startContainer.ELEMENT_NODE === 1) {
		startPoint = selectArea[0].node;
	    } else {
		throw "unsupported node type";
	    }
	    makeRangeBold(startPoint, insertPoint);
	} else {
	    let endContainer = selectArea[1].node;
	    while (endContainer.parentNode !== range.commonAncestorContainer) {
		makeRangeBold(endContainer.parentNode.firstChild, endContainer.previousSibling);
		endContainer = endContainer.parentNode;
	    }
	    let startContainer = selectArea[0].node;
	    while (startContainer.parentNode !== range.commonAncestorContainer) {
		makeRangeBold(startContainer.nextSibling, null);
		startContainer = startContainer.parentNode;
	    }
	    if (startContainer.nextSibling !== endContainer) {
		makeRangeBold(startContainer.nextSibling, endContainer)
	    }
	    
	    if (selectArea[1].node.TEXT_NODE === 3) {
		const textNode = selectArea[1].node as Text;
		const insertPoint = textNode.splitText(selectArea[1].count);
		makeRangeBold(textNode, insertPoint);
	    } else if (range.startContainer.ELEMENT_NODE === 1) {
		makeRangeBold(selectArea[1].node.parentNode.firstChild, selectArea[1].node.nextSibling);
	    } else {
		throw "unsupported node type";
	    }
	    if (selectArea[0].node.TEXT_NODE === 3) {
		const textNode = selectArea[0].node as Text;
		const text = textNode.splitText(selectArea[0].count);
		makeRangeBold(text, null);
	    } else if (range.startContainer.ELEMENT_NODE === 1) {
		makeRangeBold(selectArea[0].node, null);
	    } else {
		throw "unsupported node type";
	    }
	}
    }
}

// events block-change
// text-change
// selection-change

// apis
// get content
// set content
// insert
// format text
// selection https://developer.mozilla.org/en-US/docs/Web/API/Selection
// cursor position

// delete block
// get current block focus event
