using System;

namespace MidsApp
{
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CollectionAttribute : Attribute
    {
        /// <summary>
        /// Name of Collection
        /// </summary>
        public string? CollectionName { get; }

        /// <inheritdoc />
        public CollectionAttribute(string? collectionName)
        {
            CollectionName = collectionName;
        }
    }
}
