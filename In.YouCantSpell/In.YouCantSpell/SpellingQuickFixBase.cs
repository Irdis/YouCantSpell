using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Daemon;
using JetBrains.Util;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
#if RSHARP6
using JetBrains.ReSharper.Feature.Services.Bulbs;
#else
using JetBrains.ReSharper.Feature.Services.Bulbs;
#endif

namespace YouCantSpell.ReSharper
{
    /// <summary>
    /// The base spelling quick fix class.
    /// </summary>
    /// <typeparam name="THighlighting">The highlighting type that the quick fix will attach to.</typeparam>
    /// <typeparam name="TSpellingFixBulbItem">The spelling bulb item type that is generated by the highlighting.</typeparam>
    public abstract class SpellingQuickFixBase<THighlighting, TSpellingFixBulbItem> : IQuickFix
        where THighlighting : SpellingErrorHighlightingBase
        where TSpellingFixBulbItem : SpellingFixBulbItemBase<THighlighting>
    {

        /// <summary>
        /// The bound highlighting.
        /// </summary>
        private readonly THighlighting _highlighting;

        /// <summary>
        /// Creates a new quick fix bound to the given highlighting.
        /// </summary>
        /// <param name="highlighting">The highlighting to be bound to.</param>
        /// <remarks>
        /// The derived constructors should be called by ReSharper and the highlighting should be injected into it.
        /// </remarks>
        protected SpellingQuickFixBase([NotNull] THighlighting highlighting)
        {
            _highlighting = highlighting;
        }

        /// <summary>
        /// The highlighting that is bound to this quick fix.
        /// </summary>
        public THighlighting Highlighting { get { return _highlighting; } }

        /// <summary>
        /// Generates a single spelling fix bulb item from the given suggestion.
        /// </summary>
        /// <param name="suggestion">The suggestion text.</param>
        /// <returns>A new bulb item that can correct the highlighted spelling mistake with one suggestion.</returns>
        protected abstract TSpellingFixBulbItem CreateSpellingFix(string suggestion);


        /// <summary>
        /// Creates the spelling fix bulb items associated with the bound highlighting.
        /// </summary>
        public

        IBulbAction[]
        Items
        {
            get
            {
                // add all of the spelling suggestions
                var items = _highlighting.Suggestions
                    .Select<string,IBulbAction>((s) => CreateSpellingFix(s))
                    .Take(ReSharperUtil.MaxSuggestions)
                    .ToList();

                // append on the option parameters
                items.Add(new SpellingAddToDictionaryBulbItem(_highlighting));
                items.Add(new SpellingIgnoreWordBulbItem(_highlighting));

                return items.ToArray();
            }
        }

        public IEnumerable<IntentionAction> CreateBulbItems()
        {
            return Items.ToContextActionIntentions();
        }

        /// <summary>
        /// The quick fix is available if the highlight is valid.
        /// </summary>
        /// <param name="cache">Not used.</param>
        /// <returns>True when available.</returns>
        public bool IsAvailable(IUserDataHolder cache)
        {
            return _highlighting.IsValid();
        }

#if RSHARP7
		public void CreateBulbItems(BulbMenu menu, Severity severity)
		{
			var defaultGroup = menu.GetOrCreateGroup(Anchor.DefaultAnchor);
			var subMenu = defaultGroup.GetOrCreateSubmenu(new BulbMenuItemViewDescription(
				new Anchor("SpellingFixes", new AnchorRelation[0]),
				UnnamedThemedIcons.SpellCheckOptionsIcon.Id,
				ResourceAccessor.GetString("RS_SpellingFixesCategoryName") ?? "Spelling Fixes"
			));
			subMenu.Submenu.ArrangeQuickFixes(Items.Select(x => new Pair<IBulbAction, Severity>(x,severity)));

		}

#endif

#if RSHARP8
        public IEnumerable<IntentionAction> CreateBulbItems()
        {
          return Items.ToContextAction();
        }
#endif
    }
}
