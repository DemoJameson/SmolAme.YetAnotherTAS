﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace TAS.Studio.RichText;

/// <summary>
/// This class contains the source text (chars and styles).
/// It stores a text lines, the manager of commands, undo/redo stack, styles.
/// </summary>
public class TextSource : IList<Line>, IDisposable {
    readonly protected List<Line> lines = new();
    readonly LinesAccessor linesAccessor;

    /// <summary>
    /// Styles
    /// Maximum style count is 16
    /// </summary>
    public readonly Style[] Styles = new Style[sizeof(ushort) * 8];

    RichText currentTB;
    int lastLineUniqueId;

    public TextSource(RichText currentTB) {
        CurrentTB = currentTB;
        linesAccessor = new LinesAccessor(this);
        Manager = new CommandManager(this);
        InitDefaultStyle();
    }

    internal CommandManager Manager { get; private set; }

    /// <summary>
    /// Current focused FastColoredTextBox
    /// </summary>
    public RichText CurrentTB {
        get => currentTB;
        set {
            currentTB = value;
            OnCurrentTBChanged();
        }
    }

    /// <summary>
    /// Default text style
    /// This style is using when no one other TextStyle is not defined in Char.style
    /// </summary>
    public TextStyle DefaultStyle { get; set; }

    /// <summary>
    /// Text lines
    /// </summary>
    public IList<string> Lines => linesAccessor;

    public bool IsNeedBuildRemovedLineIds => LineRemoved != null;

    public virtual void Dispose() { }

    public virtual Line this[int i] {
        get => lines[i];
        set => throw new NotImplementedException();
    }

    public IEnumerator<Line> GetEnumerator() {
        return lines.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return (lines as IEnumerator);
    }

    public virtual int IndexOf(Line item) {
        return lines.IndexOf(item);
    }

    public virtual void Insert(int index, Line item) {
        InsertLine(index, item);
    }

    public virtual void RemoveAt(int index) {
        RemoveLine(index);
    }

    public virtual void Add(Line item) {
        InsertLine(Count, item);
    }

    public virtual void Clear() {
        RemoveLine(0, Count);
    }

    public virtual bool Contains(Line item) {
        return lines.Contains(item);
    }

    public virtual void CopyTo(Line[] array, int arrayIndex) {
        lines.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Lines count
    /// </summary>
    public virtual int Count => lines.Count;

    public virtual bool IsReadOnly => false;

    public virtual bool Remove(Line item) {
        int i = IndexOf(item);
        if (i >= 0) {
            RemoveLine(i);
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Occurs when line was inserted/added
    /// </summary>
    public event EventHandler<LineInsertedEventArgs> LineInserted;

    /// <summary>
    /// Occurs when line was removed
    /// </summary>
    public event EventHandler<LineRemovedEventArgs> LineRemoved;

    /// <summary>
    /// Occurs when text was changed
    /// </summary>
    public event EventHandler<TextChangedEventArgs> TextChanged;

    /// <summary>
    /// Occurs when recalc is needed
    /// </summary>
    public event EventHandler<TextChangedEventArgs> RecalcNeeded;

    /// <summary>
    /// Occurs before text changing
    /// </summary>
    public event EventHandler<TextChangingEventArgs> TextChanging;

    /// <summary>
    /// Occurs after CurrentTB was changed
    /// </summary>
    public event EventHandler CurrentTBChanged;

    public virtual void ClearIsChanged() {
        foreach (var line in lines) {
            line.IsChanged = false;
        }
    }

    public virtual Line CreateLine() {
        return new(GenerateUniqueLineId());
    }

    private void OnCurrentTBChanged() {
        if (CurrentTBChanged != null) {
            CurrentTBChanged(this, EventArgs.Empty);
        }
    }

    public void InitDefaultStyle() {
        DefaultStyle = new TextStyle(null, null, FontStyle.Regular);
    }

    public virtual bool IsLineLoaded(int iLine) {
        return lines[iLine] != null;
    }

    public int BinarySearch(Line item, IComparer<Line> comparer) {
        return lines.BinarySearch(item, comparer);
    }

    public int GenerateUniqueLineId() {
        return lastLineUniqueId++;
    }

    public virtual void InsertLine(int index, Line line) {
        lines.Insert(index, line);
        OnLineInserted(index);
    }

    public void OnLineInserted(int index) {
        OnLineInserted(index, 1);
    }

    public void OnLineInserted(int index, int count) {
        if (LineInserted != null) {
            LineInserted(this, new LineInsertedEventArgs(index, count));
        }
    }

    public virtual void RemoveLine(int index) {
        RemoveLine(index, 1);
    }

    public virtual void RemoveLine(int index, int count) {
        List<int> removedLineIds = new();
        //
        if (count > 0) {
            if (IsNeedBuildRemovedLineIds) {
                for (int i = 0; i < count; i++) {
                    removedLineIds.Add(this[index + i].UniqueId);
                }
            }
        }

        //
        lines.RemoveRange(index, count);

        OnLineRemoved(index, count, removedLineIds);
    }

    public void OnLineRemoved(int index, int count, List<int> removedLineIds) {
        if (count > 0) {
            if (LineRemoved != null) {
                LineRemoved(this, new LineRemovedEventArgs(index, count, removedLineIds));
            }
        }
    }

    public void OnTextChanged(int fromLine, int toLine) {
        if (TextChanged != null) {
            TextChanged(this, new TextChangedEventArgs(Math.Min(fromLine, toLine), Math.Max(fromLine, toLine)));
        }
    }

    internal void NeedRecalc(TextChangedEventArgs args) {
        if (RecalcNeeded != null) {
            RecalcNeeded(this, args);
        }
    }

    internal void OnTextChanging() {
        string temp = null;
        OnTextChanging(ref temp);
    }

    internal void OnTextChanging(ref string text) {
        if (TextChanging != null) {
            var args = new TextChangingEventArgs() {InsertingText = text};
            TextChanging(this, args);
            text = args.InsertingText;
            if (args.Cancel) {
                text = string.Empty;
            }
        }

        ;
    }

    public virtual int GetLineLength(int i) {
        return lines[i].Count;
    }

    public virtual bool LineHasFoldingStartMarker(int iLine) {
        return !string.IsNullOrEmpty(lines[iLine].FoldingStartMarker);
    }

    public virtual bool LineHasFoldingEndMarker(int iLine) {
        return !string.IsNullOrEmpty(lines[iLine].FoldingEndMarker);
    }

    public virtual void SaveToFile(string fileName, Encoding enc) {
        using (StreamWriter sw = new(fileName, false, enc)) {
            for (int i = 0; i < Count - 1; i++) {
                sw.WriteLine(lines[i].Text);
            }

            sw.Write(lines[Count - 1].Text);
        }
    }

    public class TextChangedEventArgs : EventArgs {
        public int iFromLine;
        public int iToLine;

        public TextChangedEventArgs(int iFromLine, int iToLine) {
            this.iFromLine = iFromLine;
            this.iToLine = iToLine;
        }
    }
}