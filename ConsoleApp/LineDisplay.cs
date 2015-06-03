namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class LineDisplay
    {
        private readonly int _displayStartFromTop;
        private readonly int _lineCount;
        private readonly List<string> _lines;
        private Tuple<int,int> _cursorPosition;

        public LineDisplay(int displayStartFromTop, int lineCount)
        {
            _displayStartFromTop = displayStartFromTop;
            _lineCount = lineCount;

            _lines = new List<string>(Enumerable.Repeat(string.Empty, lineCount));
        }

        public void SetLine(int id, string value)
        {
            _lines.RemoveAt(id);
            _lines.Insert(id, value);
        }

        public void UpdateLine(int id, string value)
        {
            SetLine(id, value);
            Update();
        }

        private void RememberCursorPosition()
        {
            _cursorPosition = Tuple.Create(Console.CursorLeft, Console.CursorTop);
        }

        private void RestoreCursorPosition()
        {
            Console.SetCursorPosition(_cursorPosition.Item1, _cursorPosition.Item2);
        }

        private static void ClearCurrentLine()
        {
            var emptyLineString = new String(' ', Console.WindowWidth-2);
            Console.Write(emptyLineString);
            Console.Write('\r');
        }

        public void Update()
        {
            RememberCursorPosition();
            Console.SetCursorPosition(0, _displayStartFromTop);
            _lines.ForEach(line =>
            {
                ClearCurrentLine();
                Console.WriteLine(line);
            });
            RestoreCursorPosition();
        }

        public void Hide()
        {
            RememberCursorPosition();
            for (int i = 0; i < _lines.Count; i++)
            {
                Console.SetCursorPosition(0, _displayStartFromTop + i);
                ClearCurrentLine();
            }
            RestoreCursorPosition();
        }

        public void ClearLine(int lineIndex)
        {
            _lines[lineIndex] = string.Empty;
        }
    }
}
