using System;
using System.Linq;
using Ferric.Text.Common.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ferric.Text.Common.Tests
{
    [TestClass]
    public class TokenizerTests
    {
        [TestMethod]
        public void Text_Tokenizer_TestUnicodeRegexpTokenizer()
        {
            var input = @"abc123.$ i'll";
            var tokens = new UnicodeRegexpTokenizer().Process(input).OfType<TokenSpan>().ToArray();

            Assert.AreEqual(TokenClass.Word, tokens[0].TokenClass);
            Assert.AreEqual("abc", tokens[0].Text);

            Assert.AreEqual(TokenClass.Number, tokens[1].TokenClass);
            Assert.AreEqual("123", tokens[1].Text);

            Assert.AreEqual(TokenClass.Punct, tokens[2].TokenClass);
            Assert.AreEqual(".", tokens[2].Text);

            Assert.AreEqual(TokenClass.Symbol, tokens[3].TokenClass);
            Assert.AreEqual("$", tokens[3].Text);

            Assert.AreEqual(TokenClass.Space, tokens[4].TokenClass);
            Assert.AreEqual(" ", tokens[4].Text);

            Assert.AreEqual(TokenClass.Word, tokens[5].TokenClass);
            Assert.AreEqual("i", tokens[5].Text);

            Assert.AreEqual(TokenClass.Punct, tokens[6].TokenClass);
            Assert.AreEqual("'", tokens[6].Text);

            Assert.AreEqual(TokenClass.Word, tokens[7].TokenClass);
            Assert.AreEqual("ll", tokens[7].Text);
        }
    }
}
