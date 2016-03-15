#include "networkclipboarditem.h"

NetworkClipboardItem::NetworkClipboardItem(){}

NetworkClipboardItem::NetworkClipboardItem(qint64 id, QString ch, QString b)
{
    userId = id;
    channel = ch;
    body = b;
}

qint64 NetworkClipboardItem::getUserId() const
{
    return userId;
}

QString NetworkClipboardItem::getChannel() const
{
    return channel;
}

QString NetworkClipboardItem::getBody() const
{
    return body;
}

QDataStream &operator<<(QDataStream &out, const NetworkClipboardItem &item)
{
    out << item.getUserId()
        << item.getChannel()
        << item.getBody();

    return out;
}

QDataStream &operator>>(QDataStream &in, NetworkClipboardItem &item)
{
    qint64 userid;
    QString body;
    QString channel;

    in >> userid >> channel >> body;

    item = NetworkClipboardItem(userid, channel, body);

    return in;
}
