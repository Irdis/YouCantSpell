using System;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace YouCantSpell.ReSharper
{
    public static class ProcessChildrenExt
    {
        public static void ProcessChildren<T>(this IFile file, Action<T> act) where T : ITreeNode
        {
            file.ProcessDescendants(new RecursiveElementProcessor<T>(act));
        }
    }
}