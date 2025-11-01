using System;
using System.Collections.Generic;
using System.Linq;

namespace HtmlSerializer
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector Parent { get; set; }
        public Selector Child { get; set; }

        public Selector()
        {
            Classes = new List<string>();
        }

        // פונקציה סטטית שממירה מחרוזת של selector לאוביקט
        public static Selector Parse(string selectorString)
        {
            if (string.IsNullOrWhiteSpace(selectorString))
            {
                return null;
            }

            // פירוק לפי רווחים (כל חלק = רמה בהיררכיה)
            var parts = selectorString.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            Selector root = null;
            Selector current = null;

            foreach (var part in parts)
            {
                var newSelector = new Selector();

                // פירוק החלק לפי # ו-.
                // נשתמש ב-Regex כדי למצוא את כל המרכיבים
                var remainingPart = part;

                // חיפוש ID (מתחיל ב-#)
                var idIndex = remainingPart.IndexOf('#');
                if (idIndex >= 0)
                {
                    var idEnd = remainingPart.IndexOfAny(new[] { '.', '#' }, idIndex + 1);
                    if (idEnd < 0) idEnd = remainingPart.Length;

                    newSelector.Id = remainingPart.Substring(idIndex + 1, idEnd - idIndex - 1);
                }

                // חיפוש classes (מתחילים ב-.)
                var classMatches = System.Text.RegularExpressions.Regex.Matches(remainingPart, @"\.([^#\.]+)");
                foreach (System.Text.RegularExpressions.Match match in classMatches)
                {
                    newSelector.Classes.Add(match.Groups[1].Value);
                }

                // חיפוש tag name (כל מה שלא מתחיל ב-# או .)
                var tagNameEnd = remainingPart.IndexOfAny(new[] { '#', '.' });
                if (tagNameEnd < 0) tagNameEnd = remainingPart.Length;

                var potentialTagName = remainingPart.Substring(0, tagNameEnd);
                if (!string.IsNullOrWhiteSpace(potentialTagName))
                {
                    // בדיקה אם זה tag תקין
                    if (HtmlHelper.Instance.IsValidTag(potentialTagName))
                    {
                        newSelector.TagName = potentialTagName;
                    }
                }

                // בניית ההיררכיה
                if (root == null)
                {
                    root = newSelector;
                    current = newSelector;
                }
                else
                {
                    current.Child = newSelector;
                    newSelector.Parent = current;
                    current = newSelector;
                }
            }

            return root;
        }
    }
}