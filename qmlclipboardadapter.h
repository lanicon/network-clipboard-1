#ifndef QMLCLIPBOARDADAPTER_H
#define QMLCLIPBOARDADAPTER_H

#include <QObject>
#include <QApplication>
#include <QClipboard>

class QmlClipboardAdapter : public QObject
{
    Q_OBJECT

    Q_PROPERTY(QString text READ getText WRITE setText NOTIFY textChanged)

public:
    explicit QmlClipboardAdapter(QObject *parent = 0);

    QString getText() const;
    void setText(QString text);

signals:
    void textChanged(QString newtext);

public slots:
};

#endif // QMLCLIPBOARDADAPTER_H
