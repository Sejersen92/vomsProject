// A container is just a collection of other blocks,
enum BlockType {
    Container,
    Text
}


interface Block {
    parent: Block;
    type: BlockType;
    tagType: string;
    blocks?: Block[];
    element: HTMLElement;
};

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

function moveBlock(block: Block, toBlock: Block, insertAt: number) {
    var blockIndex = block.parent.blocks.indexOf(block);
    block.parent.blocks.splice(blockIndex, 1);
    insertChildBlock(toBlock, block, insertAt);
}

function textChange() {

}

function blockFocus() {
    console.log("hello");
}

function makeBlock(parent: Block, insertAt: number, type: BlockType, tagType: string) {
    var element = document.createElement(tagType);
    var block: Block = {
        parent: parent,
        type: type,
        tagType: tagType,
        element: element
    };
    element.addEventListener("focus", blockFocus);
    if (type === BlockType.Text) {
        element.contentEditable = "true";
        element.addEventListener("input", textChange);
    } else if (type === BlockType.Container) {
        block.blocks = [];
    } else {
        throw "Invalid block type";
    }
    insertChildBlock(parent, block, insertAt);
    return block;
}

function makeEditor(element: HTMLElement) {
    var block: Block = {
        parent: null,
        type: BlockType.Container,
        tagType: element.tagName,
        element: element,
        blocks: []
    };

    return block;
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
// cursor pos

// delete block
// get current block focus event
