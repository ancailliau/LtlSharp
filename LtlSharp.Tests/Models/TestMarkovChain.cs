using System;
using NUnit.Framework;
using LtlSharp.Models;
using System.Linq;

namespace CheckMyModels.Tests.Models
{
    [TestFixture()]
    public class TestMarkovChain
    {
        static MarkovChain GetExampleFig101 ()
        {
            var mc = new MarkovChain ();
            var start = mc.AddVertex ("start");
            var ntry = mc.AddVertex ("try");
            var lost = mc.AddVertex ("lost");
            var delivered = mc.AddVertex ("delivered");
            mc.SetInitial (start, 1);
            
            mc.AddEdge (start, ntry);
            mc.AddEdge (ntry, .1d, lost);
            mc.AddEdge (lost, ntry);
            mc.AddEdge (ntry, .9d, delivered);
            mc.AddEdge (delivered, start);
            
            return mc;
        }
        
        static MarkovChain GetExampleFig102 ()
        {
            var mc = new MarkovChain ();
            var s0 = mc.AddVertex ("s0");
            var s123 = mc.AddVertex ("s123");
            var s456 = mc.AddVertex ("s456");
            var ss123 = mc.AddVertex ("s'123");
            var s23 = mc.AddVertex ("s23");
            var s45 = mc.AddVertex ("s45");
            var ss456 = mc.AddVertex ("s'456");
            var t1 = mc.AddVertex ("1");
            var t2 = mc.AddVertex ("2");
            var t3 = mc.AddVertex ("3");
            var t4 = mc.AddVertex ("4");
            var t5 = mc.AddVertex ("5");
            var t6 = mc.AddVertex ("6");
            mc.SetInitial (s0, 1);

            mc.AddEdge (s0, 1d/2, s123);
            mc.AddEdge (s0, 1d/2, s456);
            
            mc.AddEdge (s123, 1d/2, ss123);
            mc.AddEdge (s123, 1d/2, s23);
            
            mc.AddEdge (ss123, 1d/2, s123);
            mc.AddEdge (ss123, 1d/2, t1);

            mc.AddEdge (s23, 1d/2, t2);
            mc.AddEdge (s23, 1d/2, t3);
            
            mc.AddEdge (s45, 1d/2, t4);
            mc.AddEdge (s45, 1d/2, t5);
            
            mc.AddEdge (s456, 1d/2, s45);
            mc.AddEdge (s456, 1d/2, ss456);
            
            mc.AddEdge (ss456, 1d/2, s456);
            mc.AddEdge (ss456, 1d/2, t6);
            
            mc.AddEdge (t1, t1);
            mc.AddEdge (t2, t2);
            mc.AddEdge (t3, t3);
            mc.AddEdge (t4, t4);
            mc.AddEdge (t5, t5);
            mc.AddEdge (t6, t6);
            
            return mc;
        }
        
        MarkovChain GetExample (string example) {
            if (example == "101") {
                return GetExampleFig101 ();
            } else if (example == "102") {
                return GetExampleFig102 ();
            }
            throw new NotImplementedException ();
        }
        
        [TestCase (@"101", "start",     new [] { "try" })]
        [TestCase (@"101", "try",       new [] { "delivered", "lost" })]
        [TestCase (@"101", "delivered", new [] { "start" })]
        [TestCase (@"101", "lost",      new [] { "try" })]
        [TestCase (@"102", "s0",        new [] { "s123", "s456" })]
        [TestCase (@"102", "s123",      new [] { "s'123", "s23" })]
        [TestCase (@"102", "s456",      new [] { "s'456", "s45" })]
        [TestCase (@"102", "s'123",     new [] { "s123", "1" })]
        [TestCase (@"102", "s23",       new [] { "2", "3" })]
        [TestCase (@"102", "s45",       new [] { "4", "5" })]
        [TestCase (@"102", "s'456",     new [] { "s456", "6" })]
        [TestCase (@"102", "1",         new [] { "1" })]
        [TestCase (@"102", "2",         new [] { "2" })]
        [TestCase (@"102", "3",         new [] { "3" })]
        [TestCase (@"102", "4",         new [] { "4" })]
        [TestCase (@"102", "5",         new [] { "5" })]
        [TestCase (@"102", "6",         new [] { "6" })]
        public void TestSuccessors (string example, string source, string[] expected)
        {
            var mc = GetExample (example);
            var start = mc.GetVertex (source);
            
            var post = mc.Post (start);
            var expectedPost = expected.Select (v => mc.GetVertex (v));
            
            Assert.That (post.All (v => expectedPost.Contains (v)), "Not all nodes were expected.");
            Assert.That (expectedPost.All (v => post.Contains (v)), "All expected nodes not contained in post.");
        }

        [TestCase (@"101", "start",     new [] { "start", "delivered", "lost", "try" })]
        [TestCase (@"101", "try",       new [] { "start", "delivered", "lost", "try" })]
        [TestCase (@"101", "delivered", new [] { "start", "delivered", "lost", "try" })]
        [TestCase (@"101", "lost",      new [] { "start", "delivered", "lost", "try" })]
        [TestCase (@"102", "s0",        new [] { "s123", "s456", "s'123", "s23", "s45", "s'456", 
                                                 "1", "2", "3", "4", "5", "6" })]
        [TestCase (@"102", "s123",      new [] { "s123", "s'123", "s23", "1", "2", "3" })]
        [TestCase (@"102", "s456",      new [] { "s456", "s45", "s'456", "4", "5", "6" })]
        [TestCase (@"102", "s'123",     new [] { "s123", "s'123", "s23", "1", "2", "3" })]
        [TestCase (@"102", "s'456",     new [] { "s456", "s45", "s'456", "4", "5", "6" })]
        [TestCase (@"102", "s23",       new [] { "2", "3" })]
        [TestCase (@"102", "s45",       new [] { "4", "5" })]
        public void TestAllSuccessors (string example, string source, string[] expected)
        {
            var mc = GetExample (example);
            var start = mc.GetVertex (source);

            var post = mc.AllPost (start);
            var expectedPost = expected.Select (v => mc.GetVertex (v));
            
            Console.WriteLine (string.Join (", ", expectedPost.Select (v => v.Name)));
            Console.WriteLine (string.Join (", ", post.Select (v => v.Name)));

            Assert.That (post.All (v => expectedPost.Contains (v)), "Not all nodes were expected.");
            Assert.That (expectedPost.All (v => post.Contains (v)), "All expected nodes not contained in post.");
        }
    }
}

