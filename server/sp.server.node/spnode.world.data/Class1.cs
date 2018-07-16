using System;
using spnode.world.data.Utils.Guid;

namespace spnode.world.data
{
    public class IslandData : IUniversePersistentObject
    {
        /// <summary>
        /// The island identifier.
        /// Use <see cref="MiniGuid"/> to minify GUID string.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The assigned alias.
        /// </summary>
        public string Alias { get; set; }
    }

    public class PlanetData : IUniversePersistentObject
    {
        /// <summary>
        /// The planet identifier.
        /// Use <see cref="MiniGuid"/> to minify GUID string.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The assigned alias.
        /// </summary>
        public string Alias { get; set; }
    }

    public interface IUniversePersistentObject
    {
        /// <summary>
        /// The planet identifier.
        /// Use <see cref="MiniGuid"/> to minify GUID string.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// The assigned alias.
        /// </summary>
        string Alias { get; set; }
    }
}
