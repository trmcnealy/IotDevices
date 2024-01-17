using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CommandLine.CommandLine
{
    [DebuggerStepThrough]
    //[System.Runtime.Versioning.NonVersionable]
    public abstract class OptionSet<T> : IReadOnlyCollection<T>
        where T : class
    {
        private readonly HashSet<T> options = new HashSet<T>();

        public T this[string alias]
        {
            get { return options.SingleOrDefault(o => HasRawAlias(o, alias)) ?? options.Single(o => HasAlias(o, alias)); }
        }

        public int Count
        {
            get { return options.Count; }
        }

        protected OptionSet()
        {
        }

        protected OptionSet(IReadOnlyCollection<T> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            foreach (T option in options)
            {
                Add(option);
            }
        }

        internal void AddRange(IEnumerable<T> options)
        {
            foreach (T option in options)
            {
                Add(option);
            }
        }

        protected abstract bool HasAlias(T      option,
                                         string alias);

        protected abstract bool HasRawAlias(T      option,
                                            string alias);

        internal void Add(T option)
        {
            string preexistingAlias = RawAliasesFor(option).FirstOrDefault(alias => options.Any(o => HasRawAlias(o, alias)));

            if (preexistingAlias != null)
            {
                throw new ArgumentException($"Alias '{preexistingAlias}' is already in use.");
            }

            options.Add(option);
        }

        protected abstract IReadOnlyCollection<string> RawAliasesFor(T option);

        public bool Contains(string alias)
        {
            return options.Any(option => HasAlias(option, alias));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return options.GetEnumerator();
        }
    }
}