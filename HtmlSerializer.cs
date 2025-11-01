
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    public class HtmlSerializer


    {
        private readonly HtmlHelper _helper;

        public HtmlSerializer()
        {
            _helper = HtmlHelper.Instance;
        }

        // קריאה לדף אינטרנט
        public async Task<string> Load(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                var html = await response.Content.ReadAsStringAsync();
                return html;
            }
        }

        // פירוק HTML למחרוזות של תגיות
        public List<string> ParseHtmlToTokens(string html)
        {
            // Regex שמוצא תגיות HTML (מתחיל ב-< ומסתיים ב->)
            var regex = new Regex(@"<[^>]+>|[^<]+");
            var matches = regex.Matches(html);

            var tokens = new List<string>();
            foreach (Match match in matches)
            {
                var token = match.Value.Trim();
                // ניקוי רווחים וירידות שורה מיותרות
                if (!string.IsNullOrWhiteSpace(token))
                {
                    tokens.Add(token);
                }
            }

            return tokens;
        }

        // בניית עץ ה-HTML
        public HtmlElement BuildTree(List<string> tokens)
        {
            var root = new HtmlElement { Name = "root" };
            var currentElement = root;

            foreach (var token in tokens)
            {
                // אם זו לא תגית (לא מתחיל ב-<)
                if (!token.StartsWith("<"))
                {
                    if (currentElement != null && !string.IsNullOrWhiteSpace(token))
                    {
                        currentElement.InnerHtml = token;
                    }
                    continue;
                }

                // הסרת < ו-> מהתגית
                var content = token.Substring(1, token.Length - 2).Trim();

                // בדיקה אם זו תגית סוגרת
                if (content.StartsWith("/"))
                {
                    // עלייה רמה אחת למעלה בעץ
                    if (content.ToLower() == "/html")
                    {
                        break;
                    }
                    if (currentElement?.Parent != null)
                    {
                        currentElement = currentElement.Parent;
                    }
                    continue;
                }

                // פירוק התגית לשם ואטריביוטים
                var parts = Regex.Split(content, @"\s+");
                var tagName = parts[0].ToLower();

                // בדיקה אם זו תגית HTML תקינה
                if (!_helper.IsValidTag(tagName))
                {
                    continue;
                }

                // יצירת אלמנט חדש
                var newElement = new HtmlElement { Name = tagName };

                // פירוק אטריביוטים
                var attributesRegex = new Regex(@"(\w+)=""([^""]*)""");
                var attributeMatches = attributesRegex.Matches(content);

                foreach (Match attrMatch in attributeMatches)
                {
                    var attrName = attrMatch.Groups[1].Value;
                    var attrValue = attrMatch.Groups[2].Value;

                    newElement.Attributes.Add($"{attrName}=\"{attrValue}\"");

                    if (attrName.ToLower() == "id")
                    {
                        newElement.Id = attrValue;
                    }
                    else if (attrName.ToLower() == "class")
                    {
                        // פירוק classes לפי רווחים
                        var classes = attrValue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        newElement.Classes.AddRange(classes);
                    }
                }

                // הוספת האלמנט לעץ
                newElement.Parent = currentElement;
                currentElement.Children.Add(newElement);

                // בדיקה אם התגית סוגרת את עצמה
                bool isSelfClosing = content.EndsWith("/") || _helper.IsSelfClosing(tagName);

                if (!isSelfClosing)
                {
                    // מעבר לרמה הבאה בעץ
                    currentElement = newElement;
                }
            }

            return root;
        }

        // פונקציה מלאה שעושה את כל התהליך
        public async Task<HtmlElement> SerializeAsync(string url)
        {
            var html = await Load(url);
            var tokens = ParseHtmlToTokens(html);
            var tree = BuildTree(tokens);
            return tree;
        }
    }
}