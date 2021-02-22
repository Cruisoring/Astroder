using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PolygonControl
{
    partial class PolygonControl
    {
        /// <summary>
        /// 	Base class for an object which can be included in a <c>Collection&lt;T&gt;</c>.
        /// </summary>
        public class CollectionItem
        {
            /// <summary>
            /// 	Default constructor.  Sets <c>Container</c> to <c>null</c>.
            /// </summary>
            public CollectionItem()
            {
                Cc = null;
            }

            /// <summary>
            /// 	Gets or sets <c>PolygonControl</c> to which this object belongs.
            /// </summary>
            /// <value>
            /// 	<c>PolygonControl</c> to which this object belongs.
            /// </value>
            public PolygonControl Cc { get; internal set; }

            /// <summary>
            /// 	Redraw control.
            /// </summary>
            protected void Redraw()
            {
                if (Cc != null)
                {
                    Cc.Redraw();
                }
            }
        }

        /// <summary>
        /// 	Class for a collection of <c>CollectionItem</c>-derived objects.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        public class Collection<T> where T : CollectionItem
        {
            private PolygonControl cc;
            private List<T> list;

            internal Collection(PolygonControl cc)
            {
                Debug.Assert(cc != null);

                this.cc = cc;
                list = new List<T>();
            }

            /// <summary>
            /// 	Gets the <c>CollectionItem</c> at the specified index.
            /// </summary>
            /// <value>
            /// 	<c>CollectionItem</c> at the specified index.
            /// </value>
            /// <param name="index">The zero-based index of the <c>CollectionItem</c> to get.</param>
            /// <returns>
            ///		The <c>CollectionItem</c> at the specified index.
            /// </returns>
            /// <exception cref="System.ArgumentOutOfRangeException">
            ///		Thrown if <paramref name="index" /> is less than zero or greater than <c>Count</c>.
            /// </exception>
            public T this[int index]
            {
                get { return list[index]; }
            }

            /// <summary>
            /// 	Gets the number of <c>CollectionItem</c> objects contained in the <c>Collection&lt;T&gt;</c>.
            /// </summary>
            /// <value>
            /// 	The number of <c>CollectionItem</c> objects contained in the <c>Collection&lt;T&gt;</c>.
            /// </value>
            public int Count
            {
                get { return list.Count; }
            }

            internal void CheckIfNull(T t)
            {
                if (t == null)
                {
                    throw new ArgumentNullException();
                }
            }

            internal void Check(T t)
            {
                CheckIfNull(t);
                if (t.Cc != null)
                {
                    throw new ArgumentException(string.Format("Cannot add or insert the item '{0}' to more than one control.  You must first remove it from its current control or clone it.", t),
                                                "t");
                }
            }

            /// <summary>
            /// 	Returns the zero-based index of the occurence of a <c>CollectionItem</c> in the <c>Collection&lt;T&gt;</c>.
            /// </summary>
            /// <param name="t">The <c>CollectionItem</c> to locate in the <c>Collection&lt;T&gt;</c>.</param>
            /// <returns>
            ///		The zero-based index of <paramref name="t" />, if found; otherwise -1.
            ///	</returns>
            /// <exception cref="System.ArgumentNullException">
            ///		Thrown if <paramref name="t" /> is null.
            ///	</exception>
            public int IndexOf(T t)
            {
                CheckIfNull(t);
                return list.IndexOf(t);
            }

            /// <summary>
            /// 	Adds a <c>CollectionItem</c> to the end of the <c>Collection&lt;T&gt;</c>.
            /// </summary>
            /// <param name="t">The <c>CollectionItem</c> to be added to the end of the <c>Collection&lt;T&gt;</c>.</param>
            /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="t" /> is null.</exception>
            /// <exception cref="System.ArgumentException">Thrown if <paramref name="t" /> already belongs to a <c>Collection&lt;T&gt;</c>.</exception>
            public void Add(T t)
            {
                Check(t);
                t.Cc = cc;
                list.Add(t);
                cc.Redraw();
            }

            /// <summary>
            /// 	Inserts a <c>CollectionItem</c> into the <c>Collection&lt;T&gt;</c> at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index at which <paramref name="t" />should be inserted.</param>
            /// <param name="t">The <c>CollectionItem</c> to insert.</param>
            /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="t" /> is null.</exception>
            /// <exception cref="System.ArgumentException">Thrown if <paramref name="t" /> already belongs to a <c>Collection&lt;T&gt;</c>.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="index" /> is less than zero or greater than <c>Count</c>.</exception>
            public void Insert(int index, T t)
            {
                Check(t);
                if (index < 0 || index > Count)
                {
                    throw new ArgumentOutOfRangeException();
                }
                t.Cc = null;
                list.Insert(index, t);
                cc.Redraw();
            }

            private void RemoveAt2(int index)
            {
                T t = list[index];
                t.Cc = null;
                list.RemoveAt(index);
            }

            /// <summary>
            /// 	Removes the <c>CollectionItem</c> at the specified index of the <c>Collection&lt;T&gt;</c>
            /// </summary>
            /// <param name="index">The zero-based index of the <c>CollectionItem</c> to remove.</param>
            /// <exception cref="System.ArgumentOutOfRangeException">
            ///		Thrown if <paramref name="index" /> is less than zero or greater than <c>Count</c>.
            ///	</exception>
            public void RemoveAt(int index)
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }
                RemoveAt2(index);
                cc.Redraw();
            }

            /// <summary>
            /// 	Removes the occurence of the <c>CollectionItem</c> from the <c>Collection&lt;T&gt;</c>.
            /// </summary>
            /// <param name="t">The <c>CollectionItem</c> to remove from <c>Collection&lt;T&gt;</c>.</param>
            /// <exception cref="System.ArgumentNullException">
            ///		Thrown if <paramref name="t" /> is null.
            ///	</exception>
            public void Remove(T t)
            {
                RemoveAt(IndexOf(t));
            }

            /// <summary>
            /// 	Removes all the <c>CollectionItem</c> objects from the <c>Collection&lt;T&gt;</c>.
            /// </summary>
            public void Clear()
            {
                while (Count > 0)
                {
                    RemoveAt2(0);
                }
                cc.Redraw();
            }
        }
    }

}
