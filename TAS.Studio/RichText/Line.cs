﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TAS.Studio.RichText;

public class Line : IList<Char> {
    protected List<Char> chars;

    internal Line(int uid) {
        UniqueId = uid;
        chars = new List<Char>();
    }

    public string FoldingStartMarker { get; set; }
    public string FoldingEndMarker { get; set; }
    public bool IsChanged { get; set; }

    /// <summary>
    /// Time of last visit of caret in this line
    /// </summary>
    /// <remarks>This property can be used for forward/backward navigating</remarks>
    public DateTime LastVisit { get; set; }

    public Brush BackgroundBrush { get; set; }
    public int UniqueId { get; private set; }
    public int AutoIndentSpacesNeededCount { get; internal set; }

    public virtual string Text {
        get {
            StringBuilder sb = new(Count);
            foreach (Char c in this) {
                sb.Append(c.c);
            }

            return sb.ToString();
        }
    }

    public int StartSpacesCount {
        get {
            int spacesCount = 0;
            for (int i = 0; i < Count; i++) {
                if (this[i].c == ' ') {
                    spacesCount++;
                } else {
                    break;
                }
            }

            return spacesCount;
        }
    }

    public int IndexOf(Char item) {
        return chars.IndexOf(item);
    }

    public void Insert(int index, Char item) {
        chars.Insert(index, item);
    }

    public void RemoveAt(int index) {
        chars.RemoveAt(index);
    }

    public Char this[int index] {
        get => chars[index];
        set => chars[index] = value;
    }

    public void Add(Char item) {
        chars.Add(item);
    }

    public void Clear() {
        chars.Clear();
    }

    public bool Contains(Char item) {
        return chars.Contains(item);
    }

    public void CopyTo(Char[] array, int arrayIndex) {
        chars.CopyTo(array, arrayIndex);
    }

    public int Count => chars.Count;

    public bool IsReadOnly => false;

    public bool Remove(Char item) {
        return chars.Remove(item);
    }

    public IEnumerator<Char> GetEnumerator() {
        return chars.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
        return chars.GetEnumerator() as System.Collections.IEnumerator;
    }

    /// <summary>
    /// Clears style of chars, delete folding markers
    /// </summary>
    public void ClearStyle(StyleIndex styleIndex) {
        FoldingStartMarker = null;
        FoldingEndMarker = null;
        for (int i = 0; i < Count; i++) {
            Char c = this[i];
            c.style &= ~styleIndex;
            this[i] = c;
        }
    }

    public void ClearFoldingMarkers() {
        FoldingStartMarker = null;
        FoldingEndMarker = null;
    }

    public virtual void RemoveRange(int index, int count) {
        if (index >= Count) {
            return;
        }

        chars.RemoveRange(index, Math.Min(Count - index, count));
    }

    public virtual void TrimExcess() {
        chars.TrimExcess();
    }

    public virtual void AddRange(IEnumerable<Char> collection) {
        chars.AddRange(collection);
    }
}

public struct LineInfo {
    List<int> cutOffPositions;

    //Y coordinate of line on screen
    internal int startY;
    public VisibleState VisibleState;

    public LineInfo(int startY) {
        cutOffPositions = null;
        VisibleState = VisibleState.Visible;
        this.startY = startY;
    }

    public List<int> CutOffPositions {
        get {
            if (cutOffPositions == null) {
                cutOffPositions = new List<int>();
            }

            return cutOffPositions;
        }
    }

    public int WordWrapStringsCount {
        get {
            switch (VisibleState) {
                case VisibleState.Visible:
                    if (cutOffPositions == null) {
                        return 1;
                    } else {
                        return cutOffPositions.Count + 1;
                    }
                case VisibleState.Hidden: return 0;
                case VisibleState.StartOfHiddenBlock: return 1;
            }

            return 0;
        }
    }

    internal int GetWordWrapStringStartPosition(int iWordWrapLine) {
        return iWordWrapLine == 0 ? 0 : CutOffPositions[iWordWrapLine - 1];
    }

    internal int GetWordWrapStringFinishPosition(int iWordWrapLine, Line line) {
        if (WordWrapStringsCount <= 0) {
            return 0;
        }

        return iWordWrapLine == WordWrapStringsCount - 1 ? line.Count - 1 : CutOffPositions[iWordWrapLine] - 1;
    }

    public int GetWordWrapStringIndex(int iChar) {
        if (cutOffPositions == null || cutOffPositions.Count == 0) {
            return 0;
        }

        for (int i = 0; i < cutOffPositions.Count; i++) {
            if (cutOffPositions[i] > /*>=*/ iChar) {
                return i;
            }
        }

        return cutOffPositions.Count;
    }

    internal void CalcCutOffs(int maxCharsPerLine, bool allowIME, bool charWrap, Line line) {
        int segmentLength = 0;
        int cutOff = 0;
        CutOffPositions.Clear();

        for (int i = 0; i < line.Count; i++) {
            char c = line[i].c;
            if (charWrap) {
                //char wrapping
                cutOff = Math.Min(i + 1, line.Count - 1);
            } else {
                //word wrapping
                if (allowIME && isCJKLetter(c)) //in CJK languages cutoff can be in any letter
                {
                    cutOff = i;
                } else if (!char.IsLetterOrDigit(c) && c != '_') {
                    cutOff = Math.Min(i + 1, line.Count - 1);
                }
            }

            segmentLength++;

            if (segmentLength == maxCharsPerLine) {
                if (cutOff == 0 || (cutOffPositions.Count > 0 && cutOff == cutOffPositions[cutOffPositions.Count - 1])) {
                    cutOff = i + 1;
                }

                CutOffPositions.Add(cutOff);
                segmentLength = 1 + i - cutOff;
            }
        }
    }

    private bool isCJKLetter(char c) {
        int code = Convert.ToInt32(c);
        return
            (code is >= 0x3300 and <= 0x33FF) ||
            (code is >= 0xFE30 and <= 0xFE4F) ||
            (code is >= 0xF900 and <= 0xFAFF) ||
            (code is >= 0x2E80 and <= 0x2EFF) ||
            (code is >= 0x31C0 and <= 0x31EF) ||
            (code is >= 0x4E00 and <= 0x9FFF) ||
            (code is >= 0x3400 and <= 0x4DBF) ||
            (code is >= 0x3200 and <= 0x32FF) ||
            (code is >= 0x2460 and <= 0x24FF) ||
            (code is >= 0x3040 and <= 0x309F) ||
            (code is >= 0x2F00 and <= 0x2FDF) ||
            (code is >= 0x31A0 and <= 0x31BF) ||
            (code is >= 0x4DC0 and <= 0x4DFF) ||
            (code is >= 0x3100 and <= 0x312F) ||
            (code is >= 0x30A0 and <= 0x30FF) ||
            (code is >= 0x31F0 and <= 0x31FF) ||
            (code is >= 0x2FF0 and <= 0x2FFF) ||
            (code is >= 0x1100 and <= 0x11FF) ||
            (code is >= 0xA960 and <= 0xA97F) ||
            (code is >= 0xD7B0 and <= 0xD7FF) ||
            (code is >= 0x3130 and <= 0x318F) ||
            (code is >= 0xAC00 and <= 0xD7AF);
    }
}

public enum VisibleState : byte {
    Visible,
    StartOfHiddenBlock,
    Hidden
}

public enum IndentMarker {
    None,
    Increased,
    Decreased
}