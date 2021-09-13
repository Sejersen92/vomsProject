// A container is just a collection of other blocks,
export enum BlockType {
    Container,
    Text
}

export enum EditorEventType {
    TextChange = "text-change",
    BlockOrderChange = "block-selection",
    BlockSelection = "block-selection"
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

// The serializeable representation of a block tree
export interface TransferBlock {
    type: BlockType;
    tagType: string;
    properties: { [key: string]: string };
    blocks?: TransferBlock[];
    text?: string;
}

// The logical repesentation of the editor
export interface Block {
    root: Editor;
    parent: Block;
    type: BlockType;
    tagType: string;
    blocks?: Block[];
    element: HTMLElement;
    handlers: { [key: string]: ((event: EditorEvent) => void)[]; };
    editingDisabled: boolean;
};

// The root element or the editor element
export interface Editor extends Block {
    selectedBlock: Block;
    elementBlocks: WeakMap<Node, Block>;
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
	block.parent = parent;
    } else {
        throw "parent block has to be a container";
    }
}

// Move block from its parent to toBlock, inserted at insertAt.
export function moveBlock(block: Block, toBlock: Block, insertAt: number) {
    if (block.parent.editingDisabled || toBlock.editingDisabled) {
        throw "Editing is disabled";
    }
    let fromBlock = block.parent;
    let blockIndex = block.parent.blocks.indexOf(block);
    block.parent.blocks.splice(blockIndex, 1);
    insertChildBlock(toBlock, block, insertAt);
    emit(fromBlock, EditorEventType.BlockOrderChange, {
	type: EditorEventType.BlockOrderChange,
	blockOrderChange: {}
    });
    emit(toBlock, EditorEventType.BlockOrderChange, {
	type: EditorEventType.BlockOrderChange,
	blockOrderChange: {}
    });
}

// Subscribe to events with eventType on block using handler
export function subscribe(block: Block, eventType: EditorEventType, handler: (event: EditorEvent) => void) {
    if (block.handlers[eventType] === undefined) {
	block.handlers[eventType] = [];
    }
    if (block.handlers[eventType].indexOf(handler) === -1) {
	block.handlers[eventType].push(handler);
    }
}

// Unsubscribe from events with eventType on block using handler
export function unsubscribe(block: Block, eventType: EditorEventType, handler: (event: EditorEvent) => void) {
    if (block.handlers[eventType] !== undefined) {
	var index = block.handlers[eventType].indexOf(handler);
	if (index !== -1) {
	    block.handlers[eventType].splice(index, 1);
	}
    }
}

function emit(block: Block, eventType: EditorEventType, event: EditorEvent) {
    if (block.handlers[eventType] !== undefined) {
	var handlers = block.handlers[eventType];
	for (var i = 0; i < handlers.length; i++) {
	    handlers[i](event);
	}
    }
    if (block.parent !== null) {
	emit(block.parent, eventType, event);
    }
}

// Load content into block
export function loadContent(block: Block, content: TransferBlock[]) {
    if (block.editingDisabled) {
        throw "Editing is disabled";
    }

    if (block.type !== BlockType.Container) {
	throw "Can't load content into text block";
    }
    // Work from behind so index wont change
    for (let i = block.blocks.length - 1; i >= 0; i--) {
	uncheckedDeleteBlock(block.blocks[i]);
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

// Get content from block
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

// enable editing on block
export function enableEditing(block: Block) {
    block.editingDisabled = false;
    if (block.type === BlockType.Text) {
        block.element.contentEditable = "true";
    } else {
        for (let i = 0; i < block.blocks.length; i++) {
            enableEditing(block.blocks[i]);
        }
    }
}

// disable editing on block
export function disableEditing(block: Block) {
    block.editingDisabled = true;
    if (block.type === BlockType.Text) {
        block.element.contentEditable = "false";
    } else {
        for (let i = 0; i < block.blocks.length; i++) {
            disableEditing(block.blocks[i]);
        }
    }
}

// This is used to keep track the of the event handleres used on the dom node.
// We keep this weak map to avoid exposing these methods on the block object.
let eventHandlers = new WeakMap<Block, {
    textChange(): void;
    blockFocus(): void;
}>();

// Make a block with parent as it parent, inserted at insertAt, block type type, tag type tagType
export function makeBlock(parent: Block, insertAt: number, type: BlockType, tagType: string): Block{
    if (parent.editingDisabled) {
        throw "Editing is disabled";
    }

    function textChange() {
	emit(block, EditorEventType.TextChange, {
	    type: EditorEventType.TextChange,
	    textChange: {}
	});
    }
    function blockFocus() {
	block.root.selectedBlock = block;
	emit(block, EditorEventType.BlockSelection, {
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
        handlers: {},
        editingDisabled: false
    };
    block.root.elementBlocks.set(element, block);
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
    emit(parent, EditorEventType.BlockOrderChange, {
	type: EditorEventType.BlockOrderChange,
	blockOrderChange: {}
    });
    eventHandlers.set(block, {
	textChange: textChange,
	blockFocus: blockFocus
    });
    return block;
}

// Same as delete block but it will not care if editing is disabled.
// This should be the function used when deleting is part of a bigger edit.
function uncheckedDeleteBlock(block: Block) {
    if (block.type === BlockType.Container) {
	// Work from behind so index wont change
	for (let i = block.blocks.length - 1; i >= 0; i--) {
	    uncheckedDeleteBlock(block.blocks[i]);
	}
    }
    const handlers = eventHandlers.get(block);
    block.element.removeEventListener("input", handlers.textChange);
    block.element.removeEventListener("focus", handlers.blockFocus);

    let blockIndex = block.parent.blocks.indexOf(block);
    block.parent.blocks.splice(blockIndex, 1);
    block.parent.element.removeChild(block.element);

    const parent = block.parent;
    if (block.root.selectedBlock === block) {
	block.root.selectedBlock = null;
    }
    block.root = null;
    block.parent = null

    emit(parent, EditorEventType.BlockOrderChange, {
	type: EditorEventType.BlockOrderChange,
	blockOrderChange: {}
    });
}

// Delete block and child blocks
export function deleteBlock(block: Block) {
    if (block.parent.editingDisabled) {
        throw "Editing is disabled";
    }
    uncheckedDeleteBlock(block);
}

// Change the tag type for a block
export function ChagneTagType(block: Block, tagType: string) {
    if (block.parent.editingDisabled) {
        throw "Editing is disabled";
    }
    const element = document.createElement(tagType);
    while (block.element.firstChild) {
        element.appendChild(block.element.firstChild)
    }
    const handlers = eventHandlers.get(block);
    block.element.removeEventListener("input", handlers.textChange);
    block.element.removeEventListener("focus", handlers.blockFocus);

    block.parent.element.replaceChild(element, block.element);
    block.root.elementBlocks.delete(block.element);
    block.element = element;
    block.tagType = tagType;

    block.root.elementBlocks.set(element, block);
    element.addEventListener("focus", handlers.blockFocus);
    element.style.userSelect = "contain";
    if (block.type === BlockType.Text) {
        if (block.editingDisabled) {
            element.contentEditable = "false";
        } else {
            element.contentEditable = "true";
        }
        element.addEventListener("input", handlers.textChange);
    }
    emit(block.parent, EditorEventType.BlockOrderChange, {
	type: EditorEventType.BlockOrderChange,
	blockOrderChange: {}
    });
}

// Create a new editor on element
export function makeEditor(element: HTMLElement): Editor {
    let block: Editor = {
	selectedBlock: null,
        root: null,
        parent: null,
        type: BlockType.Container,
        tagType: element.tagName,
        element: element,
        blocks: [],
        handlers: {},
        editingDisabled: false,
        elementBlocks: new WeakMap()
    };
    block.root = block;
    block.selectedBlock = null;
    block.elementBlocks.set(element, block);

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

// Make the current selection of the editor bold
export function makeSelectionBlod(editor: Editor) {
    var selection = getSelection();

    for(var i = 0; i < selection.rangeCount; i++) {
	const range = selection.getRangeAt(i);
        let blockNode = range.commonAncestorContainer;
        let block = editor.elementBlocks.get(blockNode);
        while (block === undefined) {
            blockNode = blockNode.parentNode;
            if (blockNode === null) {
                continue;
            }
            block = editor.elementBlocks.get(blockNode);
        }
        if (block.editingDisabled) {
            // we can't throw here as it might effect the operation on an other valid selection.
            continue;
        }

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
        emit(block, EditorEventType.TextChange, {
	    type: EditorEventType.TextChange,
	    textChange: {}
        });
    }
}
