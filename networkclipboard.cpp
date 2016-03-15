#include "networkclipboard.h"

NetworkClipboard::NetworkClipboard(QObject *parent) : QObject(parent)
{
    userId = (qint64)rand() / RAND_MAX;

    socket.bind(APP_PORT, QUdpSocket::ShareAddress);

    connect(&socket, &QUdpSocket::readyRead,
            this, &NetworkClipboard::processMessages);
}

void NetworkClipboard::paste(QString text)
{
    if (text.length() == 0)
    {
        return;
    }

    NetworkClipboardItem item(userId, "", text);
    QByteArray datagram;
    QDataStream outstream(&datagram, QIODevice::WriteOnly);
    outstream << item;

    socket.writeDatagram(
                datagram.data(),
                datagram.size(),
                QHostAddress::Broadcast,
                APP_PORT);
}

void NetworkClipboard::processMessages()
{
    while (socket.hasPendingDatagrams())
    {
        QByteArray datagram;
        datagram.resize(socket.pendingDatagramSize());
        socket.readDatagram(datagram.data(), datagram.size());

        QDataStream stream(&datagram, QIODevice::ReadOnly);
        NetworkClipboardItem item;
        stream >> item;
        newPaste(item.getBody());
    }
}
