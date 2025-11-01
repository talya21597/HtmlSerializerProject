using System.Collections.Generic;
using System.Linq;

namespace HtmlSerializer
{
    public static class QueryExtensions
    {
        // פונקציית הרחבה לחיפוש לפי Selector
        public static IEnumerable<HtmlElement> FindElements(this HtmlElement root, string selectorString)
        {
            var selector = Selector.Parse(selectorString);
            if (selector == null)
            {
                return new List<HtmlElement>();
            }

            var results = new HashSet<HtmlElement>();
            FindElementsRecursive(root, selector, results);
            return results;
        }

        // פונקציה ריקורסיבית שמחפשת אלמנטים
        private static void FindElementsRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> results)
        {
            // תנאי עצירה - אם אין selector
            if (selector == null)
            {
                return;
            }

            // קבלת כל הצאצאים
            var descendants = element.Descendants().Skip(1); // Skip(1) כדי לדלג על האלמנט עצמו

            // סינון לפי הסלקטור הנוכחי
            var filtered = descendants.Where(e => MatchesSelector(e, selector));

            // אם זה הסלקטור האחרון (אין child) - הוספה לתוצאות
            if (selector.Child == null)
            {
                foreach (var item in filtered)
                {
                    results.Add(item);
                }
            }
            else
            {
                // המשך ריקורסיה עם הסלקטור הבא
                foreach (var item in filtered)
                {
                    FindElementsRecursive(item, selector.Child, results);
                }
            }
        }

        // פונקציה שבודקת אם אלמנט תואם לסלקטור
        private static bool MatchesSelector(HtmlElement element, Selector selector)
        {
            // בדיקת TagName
            if (!string.IsNullOrEmpty(selector.TagName))
            {
                if (element.Name?.ToLower() != selector.TagName.ToLower())
                {
                    return false;
                }
            }

            // בדיקת Id
            if (!string.IsNullOrEmpty(selector.Id))
            {
                if (element.Id != selector.Id)
                {
                    return false;
                }
            }

            // בדיקת Classes
            if (selector.Classes != null && selector.Classes.Count > 0)
            {
                if (element.Classes == null)
                {
                    return false;
                }

                foreach (var selectorClass in selector.Classes)
                {
                    if (!element.Classes.Contains(selectorClass))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // פונקציית עזר נוספת - חיפוש לפי ID בלבד (מהיר יותר)
        public static HtmlElement FindById(this HtmlElement root, string id)
        {
            return root.Descendants().FirstOrDefault(e => e.Id == id);
        }

        // פונקציית עזר נוספת - חיפוש לפי שם תגית
        public static IEnumerable<HtmlElement> FindByTagName(this HtmlElement root, string tagName)
        {
            return root.Descendants().Where(e => e.Name?.ToLower() == tagName.ToLower());
        }

        // פונקציית עזר נוספת - חיפוש לפי class
        public static IEnumerable<HtmlElement> FindByClass(this HtmlElement root, string className)
        {
            return root.Descendants().Where(e => e.Classes != null && e.Classes.Contains(className));
        }
    }
}