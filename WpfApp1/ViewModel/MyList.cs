using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 简单的自定义容器，加入事件监听
/// </summary>
namespace WpfApp1.ViewModel
{
    public class MyList<T> :List<T> where T :new()
    {
        //定义委托与事件
        public delegate void add(object o, T item);
        public delegate void insert(object o, int index, T item);
        public delegate void removeAt(object o, Int32 index);
        public delegate void remove(object o, T item);
        public delegate void clear(object o, EventArgs e);


        public event add onAdd;
        public event insert onInsert;
        public event removeAt onRemoveAt;
        public event remove onRemove;
        public event clear onClear;






        //触发
        private void fireAddEvent(T item)
        {
            onAdd?.Invoke(this, item);
        }
        private void fireInsertEvent(int index, T item)
        {
            onInsert?.Invoke(this, index, item);
        }
        private void fireRemoveAtEvent(Int32 index)
        {
            onRemoveAt?.Invoke(this, index);
        }
        private void fireRemoveEvent(T item)
        {
            onRemove?.Invoke(this, item);
        }
        private void fireClearEvent(EventArgs e)
        {
            onClear?.Invoke(this, null);
        }


        public new void Add(T item)
        {
            base.Add(item);
            fireAddEvent(item);
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            fireInsertEvent(index, item);
        }

        public new void RemoveAt(Int32 index)
        {

            try
            {
                base.RemoveAt(index);
                fireRemoveAtEvent(index);
            }
            catch(Exception e)
            {

            }
        }

        public new void Remove(T item)
        {
            //只有移除成功了，才会触发事件
            if (base.Remove(item))
            {
                fireRemoveEvent(item);
            }
        }

        public new void Clear()
        {
            base.Clear();
            fireClearEvent(null);
        }
    }
}
