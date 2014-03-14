using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text;
using Ferric.Text.Tokenizer;

namespace Ferric.Lang.English
{
    public class PorterStemmer : BaseTransducer<ISpan, ISpan>
    {
        static object my_lock = new object();
        static Stack<StemmerState> free_states = new Stack<StemmerState>();

        public PorterStemmer()
        {
        }

        public override IEnumerable<ISpan> Process(IEnumerable<ISpan> inputs)
        {
            foreach (var input in inputs)
            {
                var token = input as TokenSpan;
                if (token != null)
                {
                    token.Lemma = Stem(token.Lemma);
                }
                yield return input;
            }
        }

        /// <summary>
        /// Returns the stem of the word, according to the Porter algorithm.
        /// </summary>
        /// <param name="str">String to stem.</param>
        string Stem(string str)
        {
            if (str.Length < 3)
                return str;

            StemmerState state;
            lock (my_lock)
            {
                state = free_states.Count > 0 ? free_states.Pop() : new StemmerState();
            }

            if (str.Length > state.buf.Length)
                state.buf = new char[str.Length];
            state.last_index = state.cur_index = str.Length - 1;
            str.CopyTo(0, state.buf, 0, str.Length);

            Step1(state);
            Step2(state);
            Step3(state);
            Step4(state);
            Step5(state);
            Step6(state);

            string result = new string(state.buf, 0, state.last_index + 1);

            lock (my_lock)
            {
                free_states.Push(state);
            }

            return result;
        }

        static bool IsConsonant(char[] buf, int index)
        {
            switch (buf[index])
            {
                case 'a':
                case 'e':
                case 'i':
                case 'o':
                case 'u': return false;
                case 'y': return (index == 0) ? true : !IsConsonant(buf, index - 1);
                default: return true;
            }
        }

        static int NumConsonantSequences(char[] buf, int cur_index)
        {
            int n = 0;
            int i = 0;

            while (true)
            {
                if (i > cur_index) return n;
                if (!IsConsonant(buf, i)) break; i++;
            }
            i++;
            while (true)
            {
                while (true)
                {
                    if (i > cur_index) return n;
                    if (IsConsonant(buf, i)) break;
                    i++;
                }
                i++;
                n++;
                while (true)
                {
                    if (i > cur_index) return n;
                    if (!IsConsonant(buf, i)) break;
                    i++;
                }
                i++;
            }
        }

        static bool VowelInStem(char[] buf, int last_index)
        {
            for (int i = 0; i <= last_index; ++i)
                if (!IsConsonant(buf, i))
                    return true;
            return false;
        }

        static bool DoubleConsonant(char[] buf, int index)
        {
            if (index < 1)
                return false;
            if (buf[index] != buf[index - 1])
                return false;
            return IsConsonant(buf, index);
        }

        static bool CVC(char[] buf, int index)
        {
            if (index < 2 || !IsConsonant(buf, index) || IsConsonant(buf, index - 1) || !IsConsonant(buf, index - 2))
                return false;
            if (buf[index] == 'w' || buf[index] == 'x' || buf[index] == 'y')
                return false;
            return true;
        }

        static bool EndsWith(char[] buf, int last_index, ref int cur_index, string str)
        {
            for (int i = 0; i < str.Length; ++i)
            {
                int buf_index = (last_index + 1) - (str.Length - i);
                if (buf_index < 0 || buf[buf_index] != str[i])
                    return false;
            }
            cur_index = last_index - str.Length;
            return true;
        }

        static void SetTo(char[] buf, int start_index, out int last_index, string str)
        {
            str.CopyTo(0, buf, start_index + 1, str.Length);
            last_index = start_index + str.Length;
        }

        static void SetIfConsonants(char[] buf, int start_index, ref int last_index, string str)
        {
            if (NumConsonantSequences(buf, start_index) > 0)
                SetTo(buf, start_index, out last_index, str);
        }

        void Step1(StemmerState state)
        {
            if (state.buf[state.last_index] == 's')
            {
                if (EndsWith(state.buf, state.last_index, ref state.cur_index, "sses"))
                    state.last_index -= 2;
                else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "ies"))
                    SetTo(state.buf, state.cur_index, out state.last_index, "i");
                else if (state.buf[state.last_index - 1] != 's')
                    --state.last_index;
            }

            if (EndsWith(state.buf, state.last_index, ref state.cur_index, "eed"))
            {
                if (NumConsonantSequences(state.buf, state.cur_index) > 0)
                    --state.last_index;
            }
            else if ((EndsWith(state.buf, state.last_index, ref state.cur_index, "ed")
                      || EndsWith(state.buf, state.last_index, ref state.cur_index, "ing"))
                     && VowelInStem(state.buf, state.cur_index))
            {
                state.last_index = state.cur_index;
                if (EndsWith(state.buf, state.last_index, ref state.cur_index, "at"))
                {
                    SetTo(state.buf, state.cur_index, out state.last_index, "ate");
                }
                else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "bl"))
                {
                    SetTo(state.buf, state.cur_index, out state.last_index, "ble");
                }
                else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "iz"))
                {
                    SetTo(state.buf, state.cur_index, out state.last_index, "ize");
                }
                else if (DoubleConsonant(state.buf, state.last_index))
                {
                    --state.last_index;
                    char ch = state.buf[state.last_index];
                    if (ch == 'l' || ch == 's' || ch == 'z')
                        ++state.last_index;
                }
                else if (NumConsonantSequences(state.buf, state.cur_index) == 1 && CVC(state.buf, state.last_index))
                {
                    SetTo(state.buf, state.cur_index, out state.last_index, "e");
                }
            }
        }

        void Step2(StemmerState state)
        {
            if (EndsWith(state.buf, state.last_index, ref state.cur_index, "y") && VowelInStem(state.buf, state.cur_index))
                state.buf[state.last_index] = 'i';
        }

        void Step3(StemmerState state)
        {
            if (state.last_index == 0)
                return;

            switch (state.buf[state.last_index - 1])
            {
                case 'a':
                    if (EndsWith(state.buf, state.last_index, ref state.cur_index, "ational"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ate");
                    else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "tional"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "tion");
                    break;
                case 'c':
                    if (EndsWith(state.buf, state.last_index, ref state.cur_index, "enci"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ence");
                    else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "anci"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ance");
                    break;
                case 'e':
                    if (EndsWith(state.buf, state.last_index, ref state.cur_index, "izer"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ize");
                    break;
                case 'l':
                    if (EndsWith(state.buf, state.last_index, ref state.cur_index, "bli"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ble");
                    else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "alli"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "al");
                    else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "entli"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ent");
                    else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "eli"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "e");
                    else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "ousli"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ous");
                    break;
                case 'o':
                    if (EndsWith(state.buf, state.last_index, ref state.cur_index, "ization"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ize");
                    else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "ation"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ate");
                    else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "ator"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ate");
                    break;
                case 's':
                    if (EndsWith(state.buf, state.last_index, ref state.cur_index, "alism"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "al");
                    else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "iveness"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ive");
                    else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "fulness"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ful");
                    else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "ousness"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ous");
                    break;
                case 't':
                    if (EndsWith(state.buf, state.last_index, ref state.cur_index, "aliti"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "al");
                    else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "iviti"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ive");
                    else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "biliti"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ble");
                    break;
                case 'g':
                    if (EndsWith(state.buf, state.last_index, ref state.cur_index, "logi"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "log");
                    break;
                default:
                    break;
            }
        }

        void Step4(StemmerState state)
        {
            if (state.last_index == 0)
                return;

            switch (state.buf[state.last_index])
            {
                case 'e':
                    if (EndsWith(state.buf, state.last_index, ref state.cur_index, "icate"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ic");
                    else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "ative"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "");
                    else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "alize"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "al");
                    break;
                case 'i':
                    if (EndsWith(state.buf, state.last_index, ref state.cur_index, "iciti"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ic");
                    break;
                case 'l':
                    if (EndsWith(state.buf, state.last_index, ref state.cur_index, "ical"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "ic");
                    else if (EndsWith(state.buf, state.last_index, ref state.cur_index, "ful"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "");
                    break;
                case 's':
                    if (EndsWith(state.buf, state.last_index, ref state.cur_index, "ness"))
                        SetIfConsonants(state.buf, state.cur_index, ref state.last_index, "");
                    break;
                default:
                    break;
            }
        }

        void Step5(StemmerState state)
        {
            if (state.last_index < 1)
                return;

            switch (state.buf[state.last_index - 1])
            {
                case 'a':
                    if (!EndsWith(state.buf, state.last_index, ref state.cur_index, "al"))
                        return;
                    break;
                case 'c':
                    if (!EndsWith(state.buf, state.last_index, ref state.cur_index, "ance")
                        && !EndsWith(state.buf, state.last_index, ref state.cur_index, "ence"))
                        return;
                    break;
                case 'e':
                    if (!EndsWith(state.buf, state.last_index, ref state.cur_index, "er"))
                        return;
                    break;
                case 'i':
                    if (!EndsWith(state.buf, state.last_index, ref state.cur_index, "ic"))
                        return;
                    break;
                case 'l':
                    if (!EndsWith(state.buf, state.last_index, ref state.cur_index, "able")
                        && !EndsWith(state.buf, state.last_index, ref state.cur_index, "ible"))
                        return;
                    break;
                case 'n':
                    if (!EndsWith(state.buf, state.last_index, ref state.cur_index, "ant")
                        && !EndsWith(state.buf, state.last_index, ref state.cur_index, "ement")
                        && !EndsWith(state.buf, state.last_index, ref state.cur_index, "ment")
                        && !EndsWith(state.buf, state.last_index, ref state.cur_index, "ent"))
                        return;
                    break;
                case 'o':
                    if (!(EndsWith(state.buf, state.last_index, ref state.cur_index, "ion") && state.cur_index >= 0
                          && (state.buf[state.cur_index] == 's' || state.buf[state.cur_index] == 't'))
                        && !EndsWith(state.buf, state.last_index, ref state.cur_index, "ou"))
                        return;
                    break;
                case 's':
                    if (!EndsWith(state.buf, state.last_index, ref state.cur_index, "ism"))
                        return;
                    break;
                case 't':
                    if (!EndsWith(state.buf, state.last_index, ref state.cur_index, "ate")
                        && !EndsWith(state.buf, state.last_index, ref state.cur_index, "iti"))
                        return;
                    break;
                case 'u':
                    if (!EndsWith(state.buf, state.last_index, ref state.cur_index, "ous"))
                        return;
                    break;
                case 'v':
                    if (!EndsWith(state.buf, state.last_index, ref state.cur_index, "ive"))
                        return;
                    break;
                case 'z':
                    if (!EndsWith(state.buf, state.last_index, ref state.cur_index, "ize"))
                        return;
                    break;
                default:
                    return;
            }

            if (NumConsonantSequences(state.buf, state.cur_index) > 1)
                state.last_index = state.cur_index;
        }

        void Step6(StemmerState state)
        {
            state.cur_index = state.last_index;

            if (state.buf[state.last_index] == 'e')
            {
                int a = NumConsonantSequences(state.buf, state.cur_index);
                if (a > 1 || a == 1 && !CVC(state.buf, state.last_index - 1))
                    --state.last_index;
            }

            if (state.buf[state.last_index] == 'l' && DoubleConsonant(state.buf, state.last_index) && NumConsonantSequences(state.buf, state.cur_index) > 1)
                --state.last_index;
        }

        class StemmerState
        {
            public char[] buf = new char[64];
            public int last_index = -1;
            public int cur_index = -1;

            public override string ToString()
            {
                return new string(buf, 0, last_index + 1);
            }
        }
    }
}
