using System;

namespace _Scripts._Infrastructure.UI
{
    public interface IPanel
    {
        void Open();
        void Close(Action onClosed = null);
    }
}