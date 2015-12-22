using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Inference
{
    class Program
    {
        static void Main(string[] args)
        {
            var stringCollection = new[] { "one", "two", "three" }.Define().As<StringCollectionContainer>();
            Console.WriteLine(stringCollection);
            Console.ReadKey();
        }
    }

    public class StringCollectionContainer : CollectionContainer<string>, IEnumerable<string>
    {
        public IEnumerator<string> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            return this.DefaultIfEmpty(string.Empty).Aggregate((sum, next) => sum + " -> " + next);
        }
    }

    /// <summary>
    /// Allows to define a collection of items as a type with a specific name.
    /// </summary>
    /// <remarks>
    /// TODO: Should it return the same reference of IEnumerable or create an internal list?
    /// </remarks>
    public abstract class CollectionContainer<TItem> : ICollectionContainer<TItem>
    {
        private IList<TItem> items;

        public IEnumerable<TItem> Items
        {
            protected get { return this.items ?? Enumerable.Empty<TItem>(); }
            set { this.items = value as IList<TItem> ?? (value ?? Enumerable.Empty<TItem>()).ToList(); }
        }
    }

    public interface ICollectionContainer<in TItem>
    {
        IEnumerable<TItem> Items { set; }
    }

    public static class EnumerableExtensions
    {
        public static IDefinable<TItem> Define<TItem>(this IEnumerable<TItem> setOfItems)
        {
            return new Definable<TItem>(setOfItems);
        }

        public interface IDefinable<out TItem>
        {
            TCollection As<TCollection>() where TCollection : ICollectionContainer<TItem>, new();
        }

        private class Definable<TItem> : IDefinable<TItem>
        {
            private readonly IEnumerable<TItem> items;

            public Definable(IEnumerable<TItem> items)
            {
                this.items = items;
            }

            public TCollection As<TCollection>() where TCollection : ICollectionContainer<TItem>, new()
            {
                return new TCollection() { Items = this.items };
            }
        }
    }
}
