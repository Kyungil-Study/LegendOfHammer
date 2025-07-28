using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Jay
{
    public static class ModifiableValueExtensions
    {
        /// <summary>
        /// 지정한 태그를 가진 Modifier들을 반환
        /// </summary>
        public static IEnumerable<IValueModifier<T>> WithTag<T>(this ModifiableValue<T> modifiableValue, [NotNull] string tag) where T : struct
        {
            return modifiableValue.GetAllModifiers().Where(modifier =>
                modifier is IModifierInfo meta &&
                string.Equals(meta.Tag, tag, StringComparison.Ordinal));
        }

        /// <summary>
        /// 지정한 태그를 가진 Modifier들을 제거
        /// </summary>
        public static void RemoveModifierWithTag<T>(this ModifiableValue<T> modifiableValue, [NotNull] string tag) where T : struct
        {
            var toRemove = modifiableValue.WithTag(tag);

            foreach (var mod in modifiableValue.GetAllModifiers())
            {
                modifiableValue.RemoveModifier(mod);
            }
        }

        /// <summary>
        /// 지정한 태그를 가진 Modifier들의 개수를 반환
        /// </summary>
        public static int CountTag<T>(this ModifiableValue<T> modifiableValue, [NotNull] string tag) where T : struct
        {
            return modifiableValue.WithTag(tag).Count();
        }

        /// <summary>
        /// 지정한 Source에서 유래한 Modifier들을 반환
        /// </summary>
        public static IEnumerable<IValueModifier<T>> FromSource<T>(this ModifiableValue<T> modifiableValue, [NotNull] object source) where T : struct
        {
            return modifiableValue.GetAllModifiers().Where(modifier =>
                modifier is IModifierInfo meta &&
                Equals(meta.Source, source));
        }

        /// <summary>
        /// Identifier가 일치하는 Modifier를 반환 (없으면 null)
        /// </summary>
        public static IValueModifier<T> WithIdentifier<T>(this ModifiableValue<T> modifiableValue, [NotNull] string identifier) where T : struct
        {
            return modifiableValue.GetAllModifiers().FirstOrDefault(modifier =>
                modifier is IModifierInfo meta &&
                string.Equals(meta.Identifier, identifier, StringComparison.Ordinal));
        }

        /// <summary>
        /// Identifier가 일치하는 Modifier가 존재하는지 조회
        /// </summary>
        public static bool ContainsIdentifier<T>(this ModifiableValue<T> modifiableValue, [NotNull] string identifier) where T : struct
        {
            return modifiableValue.GetAllModifiers().Any(mod => 
                mod is IModifierInfo meta && string.Equals(meta.Identifier, identifier, StringComparison.Ordinal));
        }

        
        /// <summary>
        /// 모든 Modifier의 Identifier를 반환 (없으면 null)
        /// </summary>
        public static IEnumerable<string> GetAllIdentifier<T>(this ModifiableValue<T> modifiableValue) where T : struct
        {
            return modifiableValue.GetAllModifiers().Select(mod => mod.Identifier);
        }
        
        
    }
}

