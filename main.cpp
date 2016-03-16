#include <QApplication>
#include <QQmlApplicationEngine>
#include <QtQml>

#include "networkclipboard.h"
#include "qmlclipboardadapter.h"

// fix for mingw bug 2250
#if defined(_WIN32) || defined(_WIN64)
	#define __NO_INLINE__
#endif

int main(int argc, char *argv[])
{
    QTime time = QTime::currentTime();
    srand(time.msec());

    QApplication app(argc, argv);

    QQmlApplicationEngine engine;

    qmlRegisterType<NetworkClipboard>("networkclipboard", 1, 0, "NetworkClipboard");
    qmlRegisterType<QmlClipboardAdapter>("networkclipboard", 1, 0, "Clipboard");

    engine.load(QUrl(QStringLiteral("qrc:/main.qml")));

    return app.exec();
}
