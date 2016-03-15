TEMPLATE = app

QT += qml quick widgets

CONFIG += c++11

SOURCES += main.cpp \
    networkclipboard.cpp \
    networkclipboarditem.cpp \
    qmlclipboardadapter.cpp

RESOURCES += qml.qrc

# Additional import path used to resolve QML modules in Qt Creator's code model
QML_IMPORT_PATH =

# Default rules for deployment.
include(deployment.pri)

HEADERS += \
    networkclipboard.h \
    networkclipboarditem.h \
    qmlclipboardadapter.h

DESTDIR = bin
OBJECTS_DIR = build
MOC_DIR = build
RCC_DIR = build
UI_DIR = build
