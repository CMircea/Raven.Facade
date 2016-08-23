using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Client.Connection.Async;

namespace Raven.Facade
{
    public sealed class RavenSessionCommands
    {
        private readonly IAsyncDatabaseCommands _commands;

        /*\ ***** ***** ***** ***** ***** Properties ***** ***** ***** ***** ***** \*/
        public RavenSessionSetCommands Set => new RavenSessionSetCommands(_commands);

        /*\ ***** ***** ***** ***** ***** Constructor ***** ***** ***** ***** ***** \*/
        public RavenSessionCommands(IAsyncDatabaseCommands commands)
        {
            if (commands == null)
                throw new ArgumentNullException(nameof(commands));

            _commands = commands;
        }

        /*\ ***** ***** ***** ***** ***** Public Methods ***** ***** ***** ***** ***** \*/
        public async Task<BatchResult> ExecuteAsync(ICommandData command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var result = await ExecuteAsync(new[] { command });

            return result.Single();
        }

        public async Task<IList<BatchResult>> ExecuteAsync(params ICommandData[] commands)
        {
            if (commands == null)
                throw new ArgumentNullException(nameof(commands));

            var result = await _commands.BatchAsync(commands);

            return result.ToList();
        }

        public Task<IList<BatchResult>> ExecuteAsync(IEnumerable<ICommandData> commands)
        {
            if (commands == null)
                throw new ArgumentNullException(nameof(commands));

            return ExecuteAsync(commands.ToArray());
        }

        public async Task<bool> ExistsAsync(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var metadata = await _commands.HeadAsync(id);

            return metadata != null;
        }

        public async Task<RavenMetadata> LoadMetadataAsync(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var metadata = await _commands.HeadAsync(id);

            if (metadata == null)
            {
                return null;
            }
            else
            {
                return new RavenMetadata(metadata);
            }
        }

        public Task<IList<RavenMetadata>> LoadMetadataAsync(params string[] ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            return LoadMetadataAsync((IEnumerable<string>) ids);
        }

        public async Task<IList<RavenMetadata>> LoadMetadataAsync(IEnumerable<string> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _commands.GetAsync(ids.Distinct().ToArray(), includes: null, metadataOnly: true);

            return result.Results.Select(meta => new RavenMetadata(meta)).ToList();
        }

        public Task DeleteAsync(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return _commands.DeleteAsync(id, null);
        }

        public Task PatchAsync(string id, PatchRequest patch)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (patch == null)
                throw new ArgumentNullException(nameof(patch));

            return _commands.PatchAsync(id, new[] { patch }, ignoreMissing: false);
        }

        public Task PatchAsync(string id, PatchRequest patch, Etag etag)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (patch == null)
                throw new ArgumentNullException(nameof(patch));

            return _commands.PatchAsync(id, new[] { patch }, etag);
        }

        public Task PatchAsync(string id, PatchRequest patch, bool ignoreMissing)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (patch == null)
                throw new ArgumentNullException(nameof(patch));

            return _commands.PatchAsync(id, new[] { patch }, ignoreMissing);
        }

        public Task PatchAsync(string id, ScriptedPatchRequest patch)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (patch == null)
                throw new ArgumentNullException(nameof(patch));

            return _commands.PatchAsync(id, patch, ignoreMissing: false);
        }

        public Task PatchAsync(string id, ScriptedPatchRequest patch, Etag etag)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (patch == null)
                throw new ArgumentNullException(nameof(patch));

            return _commands.PatchAsync(id, patch, etag);
        }

        public Task PatchAsync(string id, ScriptedPatchRequest patch, bool ignoreMissing)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (patch == null)
                throw new ArgumentNullException(nameof(patch));

            return _commands.PatchAsync(id, patch, ignoreMissing);
        }

        public Task PatchAsync(string id, params PatchRequest[] patches)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (patches == null)
                throw new ArgumentNullException(nameof(patches));

            return _commands.PatchAsync(id, patches, ignoreMissing: false);
        }

        public Task PatchAsync(string id, IEnumerable<PatchRequest> patches)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (patches == null)
                throw new ArgumentNullException(nameof(patches));

            return PatchAsync(id, patches.ToArray());
        }

        public Task PatchAsync(string id, IEnumerable<PatchRequest> patches, Etag etag)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (patches == null)
                throw new ArgumentNullException(nameof(patches));

            return _commands.PatchAsync(id, patches.ToArray(), etag);
        }

        public Task PatchAsync(string id, IEnumerable<PatchRequest> patches, bool ignoreMissing)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (patches == null)
                throw new ArgumentNullException(nameof(patches));

            return PatchAsync(id, patches.ToArray(), ignoreMissing);
        }
    }
}
