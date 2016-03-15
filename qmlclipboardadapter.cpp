#include "qmlclipboardadapter.h"

QmlClipboardAdapter::QmlClipboardAdapter(QObject *parent) : QObject(parent)
{

}

QString QmlClipboardAdapter::getText() const
{
    return QApplication::clipboard()->text();
}

void QmlClipboardAdapter::setText(QString text)
{
    QApplication::clipboard()->setText(text);
    emit textChanged(text);
}
