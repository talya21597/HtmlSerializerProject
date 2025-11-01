using System.Collections.Generic;
using System.Linq;

namespace HtmlSerializer
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }

        public HtmlElement()
        {
            Attributes = new List<string>();
            Classes = new List<string>();
            Children = new List<HtmlElement>();
        }

        // פונקציה שמחזירה את כל הצאצאים (מכל הדורות) - באמצעות Queue
        public IEnumerable<HtmlElement> Descendants()
        {
            var queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;

                foreach (var child in current.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        // פונקציה שמחזירה את כל האבות (מכל הדורות)
        public IEnumerable<HtmlElement> Ancestors()
        {
            var current = this.Parent;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

        // Override של GetHashCode ו-Equals כדי ש-HashSet יוכל לזהות כפילויות
        public override bool Equals(object obj)
        {
            if (obj is HtmlElement other)
            {
                return ReferenceEquals(this, other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}