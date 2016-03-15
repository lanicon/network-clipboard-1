#ifndef NETWORKCLIPBOARD_H
#define NETWORKCLIPBOARD_H

#include <QObject>
#include <QtNetwork>
#include <QDataStream>

#include "networkclipboarditem.h"

#define APP_PORT 45454

class NetworkClipboard : public QObject
{
    Q_OBJECT

private:
    QUdpSocket socket;
    qint64 userId;

public:
    explicit NetworkClipboard(QObject *parent = 0);

    Q_INVOKABLE void paste(QString text);

signals:
    void newPaste(QString text);

public slots:
    void processMessages();
};

#endif // NETWORKCLIPBOARD_H
