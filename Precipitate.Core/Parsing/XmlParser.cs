using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sprache;

namespace Precipitate.Parsing
{
    public class Document
    {
        public Node Root;

        public override string ToString()
        {
            return Root.ToString();
        }
    }

    public class Item { }

    public class Content : Item
    {
        public string Text;

        public override string ToString()
        {
            return Text;
        }
    }

    public class Node : Item
    {
        public string Name;
        public IDictionary<string, string> Attributes;
        public Node Parent;
        public IEnumerable<Item> Children;

        public override string ToString()
        {
            string attributes = String.Empty;
            if (Attributes != null && Attributes.Count > 0)
            {
                attributes = Attributes.Aggregate("", (s, c) => s + " " + c.Key + "=\"" + c.Value + "\"");
            }

            if (Children != null)
                return string.Format("<{0}{1}>\n", Name, attributes) +
                    Children.Aggregate("", (s, c) => s + c + "\n") +
                    string.Format("</{0}>", Name);

            return string.Format("<{0}{1}/>\n", Name, attributes);
        }
    }

    public class Comment : Node
    {
        public string Text;

        public override string ToString()
        {
            return "<!-- " + Text + " -->";
        }
    }

    public class NodeName
    {
        public string Identifier;
        public IDictionary<string, string> Attributes;

        public override string ToString()
        {
            if (Attributes != null && Attributes.Count > 0)
            {
                return Identifier + Attributes.Aggregate("", (s, c) => s + " " + c.Key + "=\"" + c.Value + "\"");
            }

            return Identifier;
        }
    }

    public static class XmlParser
    {
        private static readonly Parser<string> Identifier =
            from first in Parse.Letter.Once().Text()
            from rest in Parse.LetterOrDigit.XOr(Parse.Char('-')).XOr(Parse.Char('_')).Many().Text()
            select first + rest;

        private static readonly Parser<KeyValuePair<string, string>> NodeAttribute =
            from leading in Parse.WhiteSpace.Many()
            from key in Identifier
            from ws1 in Parse.WhiteSpace.Many().Until(Parse.Char('='))
            from ws2 in Parse.WhiteSpace.Many()
            from openingQuote in Parse.Char('"')
            from value in Parse.AnyChar.Until(Parse.Char('"')).Text()
            select new KeyValuePair<string, string>(key, value);

        private static readonly Parser<NodeName> NodeName =
            from identifier in Identifier
            from attributes in NodeAttribute.Many()
            select new NodeName {Identifier = identifier, Attributes = attributes.ToDictionary(k => k.Key, v => v.Value)};

        static Parser<T> Tag<T>(Parser<T> content)
        {
            return 
                   from lt in Parse.Char('<')
                   from t in content
                   from gt in Parse.Char('>').Token()
                   select t;
        }

        static readonly Parser<NodeName> BeginTag = Tag(NodeName);

        static Parser<string> EndTag(string name)
        {
            return Tag(
                       from slash in Parse.Char('/')
                       from id in Identifier
                       where id == name
                       select id).Named("closing tag for " + name);
        }

        static readonly Parser<Content> Content =
            from chars in Parse.CharExcept('<').Many()
            select new Content { Text = new string(chars.ToArray()) };

        static Parser<Node> FullNode(Node parent)
        {
            var n = new Node();

            Func<string, Node, IDictionary<string, string>, IEnumerable<Item>, Node> makeNode =
                (name, p, attrs, children) =>
                {
                    n.Name = name;
                    n.Parent = p;
                    n.Attributes = attrs;
                    n.Children = children;

                    return n;
                };

            return
                from tag in BeginTag
                from ws in Parse.WhiteSpace.Many()
                from nodes in Parse.Ref(() => Item(n)).Many()
                from ws2 in Parse.WhiteSpace.Many()
                from end in EndTag(tag.Identifier)
                select makeNode(tag.Identifier, parent, tag.Attributes, nodes);
        }

        static Parser<Node> ShortNode(Node parent)
        {
            return Tag(
                from id in NodeName
                from ws in Parse.WhiteSpace.Many()
                from slash in Parse.Char('/')
                select new Node {Name = id.Identifier, Parent = parent, Attributes = id.Attributes});
        }

        private static readonly Parser<Node> Comment = Tag(from opening in Parse.String("!--")
                                                           from comment in Parse.AnyChar.Until(Parse.String("--")).Text()
                                                           select new Comment {Text = comment});

        private static Parser<Node> Node(Node parent)
        {
            return
                from leading in Parse.WhiteSpace.Many()
                from node in ShortNode(parent).Or(FullNode(parent)).Or(Comment)
                select node;
        }

        static Parser<Item> Item(Node parent)
        {
            return
                Node(parent).Select(n => (Item) n).XOr(Content);
        }

        private static readonly Parser<string> XmlHeader =
            from openingTag in Parse.String("<?xml").Text()
            from ws in Parse.WhiteSpace.Many()
            from attributes in NodeAttribute.Many()
            from endingTag in Parse.String("?>")
            select openingTag;

        public static readonly Parser<Document> Document =
            from leading in Parse.WhiteSpace.Many()
            from hader in XmlHeader
            from ws in Parse.WhiteSpace.Many()
            from doc in Node(null).Select(n => new Document { Root = n }).End()
            select doc;
    }
}
