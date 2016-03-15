import QtQuick 2.3
import QtQuick.Controls 1.4

import networkclipboard 1.0

ApplicationWindow {
    visible: true
    width: 300
    height: 400
    title: qsTr("Network clipboard")

    NetworkClipboard {
        id: networkClipboard;
        onNewPaste: {
            logBox.text += text + '\n';
            logBox.cursorPosition = logBox.text.length - 1;
        }
    }

    Clipboard {
        id: clipboard;
    }

    menuBar: MenuBar {
        Menu {
            title: qsTr("File")
            MenuItem {
                text: qsTr("&Paste");
                shortcut: "Ctrl+V";
                onTriggered: networkClipboard.paste(clipboard.text);
            }
            MenuItem {
                text: qsTr("Exit")
                onTriggered: Qt.quit();
                shortcut: "Ctrl+Q";
            }
        }
    }

    TextArea {
        anchors.top: parent.top;
        anchors.left: parent.left;
        anchors.right: parent.right;
        anchors.bottom: parent.bottom;

        id: logBox;
        textFormat: Text.PlainText;
    }
}
