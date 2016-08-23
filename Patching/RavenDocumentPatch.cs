using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Imports.Newtonsoft.Json;
using Raven.Json.Linq;

namespace Raven.Facade.Patching
{
    public sealed class RavenDocumentPatch<T>
    {
        private readonly string _documentId;
        private readonly JsonSerializer _serializer;
        private readonly IList<PatchRequest> _patches;

        /*\ ***** ***** ***** ***** ***** Constructor ***** ***** ***** ***** ***** \*/
        internal RavenDocumentPatch([NotNull] string documentId, [NotNull] JsonSerializer serializer)
        {
            if (documentId == null)
                throw new ArgumentNullException(nameof(documentId));

            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            _documentId = documentId;
            _serializer = serializer;

            _patches = new List<PatchRequest>();
        }

        /*\ ***** ***** ***** ***** ***** Public Methods ***** ***** ***** ***** ***** \*/
        [Pure]
        [NotNull]
        public RavenDocumentPatch<T> Add<TProperty>([NotNull] Expression<Func<T, IEnumerable<TProperty>>> selector, [NotNull] object value)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return Add(selector, RavenJToken.FromObject(value, _serializer));
        }

        [Pure]
        [NotNull]
        public RavenDocumentPatch<T> Add<TProperty>([NotNull] Expression<Func<T, IEnumerable<TProperty>>> selector, [NotNull] RavenJToken value)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var expression = GetMemberExpression(selector);
            var properties = GetPropertiesFromExpression(expression).ToList();

            if (properties.Count == 1)
            {
                _patches.Add(new PatchRequest
                {
                    Name  = properties[0],
                    Type  = PatchCommandType.Add,
                    Value = value,
                });
            }
            else
            {
                var patch = new PatchRequest
                {
                    Name = properties[0],
                    Type = PatchCommandType.Modify,
                };

                _patches.Add(patch);

                for (int i = 1; i < properties.Count; i++)
                {
                    var nested = new PatchRequest
                    {
                        Name = properties[i],
                    };

                    if (i < properties.Count - 1)
                    {
                        nested.Type = PatchCommandType.Modify;
                    }
                    else
                    {
                        nested.Type  = PatchCommandType.Add;
                        nested.Value = value;
                    }

                    patch.Nested = new[] { nested };
                    patch = nested;
                }
            }

            return this;
        }

        [Pure]
        [NotNull]
        public RavenDocumentPatch<T> Set([NotNull] Expression<Func<T, object>> selector, [NotNull] object value)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return Set(selector, RavenJToken.FromObject(value, _serializer));
        }

        [Pure]
        [NotNull]
        public RavenDocumentPatch<T> Set([NotNull] Expression<Func<T, object>> selector, [NotNull] RavenJToken value)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return Set(selector, value, previousValue: null);
        }

        [Pure]
        [NotNull]
        public RavenDocumentPatch<T> Set([NotNull] Expression<Func<T, object>> selector, [NotNull] object value, [CanBeNull] object previousValue)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var valueToken    = RavenJToken.FromObject(value, _serializer);
            var previousToken = previousValue == null ? null : RavenJToken.FromObject(previousValue, _serializer);

            return Set(selector, valueToken, previousToken);
        }

        [Pure]
        [NotNull]
        public RavenDocumentPatch<T> Set([NotNull] Expression<Func<T, object>> selector, [NotNull] RavenJToken value, [CanBeNull] RavenJToken previousValue)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var expression = GetMemberExpression(selector);
            var properties = GetPropertiesFromExpression(expression).ToList();

            if (properties.Count == 1)
            {
                _patches.Add(new PatchRequest
                {
                    Name    = properties[0],
                    Type    = PatchCommandType.Set,
                    Value   = value,
                    PrevVal = previousValue,
                });
            }
            else
            {
                var patch = new PatchRequest
                {
                    Name = properties[0],
                    Type = PatchCommandType.Modify,
                };

                _patches.Add(patch);

                for (int i = 1; i < properties.Count; i++)
                {
                    var nested = new PatchRequest
                    {
                        Name = properties[i],
                    };

                    if (i < properties.Count - 1)
                    {
                        nested.Type = PatchCommandType.Modify;
                    }
                    else
                    {
                        nested.Type    = PatchCommandType.Set;
                        nested.Value   = value;
                        nested.PrevVal = previousValue;
                    }

                    patch.Nested = new[] { nested };
                    patch = nested;
                }
            }

            return this;
        }

        [Pure]
        [NotNull]
        public RavenDocumentPatch<T> Unset([NotNull] Expression<Func<T, object>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return Unset(selector, previousValue: null);
        }

        [Pure]
        [NotNull]
        public RavenDocumentPatch<T> Unset([NotNull] Expression<Func<T, object>> selector, [CanBeNull] object previousValue)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var previousToken = previousValue == null ? null : RavenJToken.FromObject(previousValue, _serializer);

            return Unset(selector, previousToken);
        }

        [Pure]
        [NotNull]
        public RavenDocumentPatch<T> Unset([NotNull] Expression<Func<T, object>> selector, [CanBeNull] RavenJToken previousValue)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var expression = GetMemberExpression(selector);
            var properties = GetPropertiesFromExpression(expression).ToList();

            if (properties.Count == 1)
            {
                _patches.Add(new PatchRequest
                {
                    Name    = properties[0],
                    Type    = PatchCommandType.Unset,
                    PrevVal = previousValue,
                });
            }
            else
            {
                var patch = new PatchRequest
                {
                    Name = properties[0],
                    Type = PatchCommandType.Modify,
                };

                _patches.Add(patch);

                for (int i = 1; i < properties.Count; i++)
                {
                    var nested = new PatchRequest
                    {
                        Name = properties[i],
                    };

                    if (i < properties.Count - 1)
                    {
                        nested.Type = PatchCommandType.Modify;
                    }
                    else
                    {
                        nested.Type    = PatchCommandType.Unset;
                        nested.PrevVal = previousValue;
                    }

                    patch.Nested = new[] { nested };
                    patch = nested;
                }
            }

            return this;
        }

        [NotNull]
        public ICommandData Build()
        {
            return Build(etag: null);
        }

        [NotNull]
        public ICommandData Build([CanBeNull] Etag etag)
        {
            return new PatchCommandData
            {
                Key     = _documentId,
                Etag    = etag,
                Patches = _patches.ToArray(),
            };
        }

        /*\ ***** ***** ***** ***** ***** Private Methods ***** ***** ***** ***** ***** \*/
        [Pure]
        [NotNull]
        private static MemberExpression GetMemberExpression<TMember>([NotNull] Expression<Func<T, TMember>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var member = selector.Body as MemberExpression;
            var unary  = selector.Body as UnaryExpression;

            if (member == null && unary != null)
                member = unary.Operand as MemberExpression;

            if (member == null)
                throw new ArgumentOutOfRangeException(nameof(selector), selector.ToString(), "The expression is not a property selector expression.");

            return member;
        }

        [Pure]
        [NotNull]
        private static IEnumerable<string> GetPropertiesFromExpression([NotNull] MemberExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (!(expression.Member is PropertyInfo))
                throw new ArgumentOutOfRangeException(nameof(expression), expression.ToString(), "The expression is not a property selector expression.");

            var property = (PropertyInfo) expression.Member;
            var nested   = expression.Expression as MemberExpression;

            if (nested != null)
            {
                foreach (string p in GetPropertiesFromExpression(nested))
                    yield return p;
            }

            yield return property.Name;
        }
    }
}
