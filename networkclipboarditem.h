#ifndef NETWORKCLIPBOARDITEM_H
#define NETWORKCLIPBOARDITEM_H

#include <QObject>
#include <QDataStream>

class NetworkClipboardItem
{
private:
    qint64 userId;
    QString body;
    QString channel;

public:
    NetworkClipboardItem();
    NetworkClipboardItem(qint64 id, QString ch, QString b);

    qint64 getUserId() const;
    QString getChannel() const;
    QString getBody() const;

    friend QDataStream &operator<<(QDataStream &out, const NetworkClipboardItem &item);
    friend QDataStream &operator>>(QDataStream &in, NetworkClipboardItem &item);
};

#endif // NETWORKCLIPBOARDITEM_H
