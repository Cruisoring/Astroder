using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PolygonControl
{
    /// <summary>
    /// Collection of GraphItems with same Border Filling fashion. 
    /// </summary>
    /// <typeparam name="T">Type of the GraphItem</typeparam>
    public abstract class GraphicItemSet<T> : IDisposable, IEnumerable<T>
        where T : GraphicItem
    {
        #region Properties
        protected PolygonControl Container;

        public List<T> Items { get; set; }

        public T this[int index] { get { return Items[index]; } }

        public int Count
        {
            get { return Items.Count; }
        }

        public List<IDisposable> Toolkit { get; set; }

        public ToPhysicalAngleDelegate Mapper { get; set; }

        #endregion

        #region Constructors

        internal GraphicItemSet(PolygonControl cc)
        {
            this.Container = cc;
            Items = new List<T>();
            Toolkit = new List<IDisposable>();
            Mapper = new ToPhysicalAngleDelegate(Container.PhysicalAngleOf);
        }

        #endregion

        #region Functions

        protected virtual void Add(T t) { Items.Add(t); }

        protected void Insert(int index, T t)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            Items.Insert(index, t);
        }

        protected void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            Items.RemoveAt(index);
        }

        protected int IndexOf(T t)
        {
            if (Items.Contains(t))
                return Items.IndexOf(t);

            return -1;
        }

        protected void Remove(T t)
        {
            int index = IndexOf(t);

            if (index != -1)
                RemoveAt(index);
        }

        /// <summary>
        /// 	Removes all the <c>CollectionItem</c> objects from the <c>Collection&lt;T&gt;</c>.
        /// </summary>
        public virtual void Clear()
        {
            Items.Clear();
            Dispose();
        }

        //public virtual void Draw(Graphics g)
        //{
        //    foreach (GraphicItem item in Items)
        //    {
        //        item.Draw(g, Container.UnitSize, 0);
        //    }
        //}

        #endregion

        #region IDisposable 成员

        public virtual void Dispose()
        {
            for (int i = 0; i < Toolkit.Count; i ++)
            {
                IDisposable tool = Toolkit[i];
                Toolkit[i] = null;
                tool.Dispose();
            }
            Toolkit.Clear();
        }

        #endregion

        #region IEnumerable<T> 成员

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T t in Items)
            {
                yield return t;
            }
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
