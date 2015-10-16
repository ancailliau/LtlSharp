using System;
using NUnit.Framework;
using LtlSharp.Models;
using System.Linq;
using LtlSharp;
using System.Collections.Generic;
using LtlSharp.Automata;
using LtlSharp.Automata.Nodes.Factories;


namespace LtlSharp.Tests.Models
{
    [TestFixture()]
    public class TestMarkovChain
    {
        public static MarkovChain<AutomatonNode> GetExampleFig101 ()
        {
            var p1 = new Proposition ("start");
            var p2 = new Proposition ("try");
            var p3 = new Proposition ("lost");
            var p4 = new Proposition ("delivered");
            
            var np1 = new Negation (p1);
            var np2 = new Negation (p2);
            var np3 = new Negation (p3);
            var np4 = new Negation (p4);
            
            var mc = new MarkovChain<AutomatonNode> (new AutomatonNodeFactory ());
            var start = mc.AddNode ("start", new ILiteral[] { p1, np2, np3, np4 });
            var ntry = mc.AddNode ("try", new ILiteral[] { np1, p2, np3, np4 });
            var lost = mc.AddNode ("lost", new ILiteral[] { np1, p2, p3, np4 });
            var delivered = mc.AddNode ("delivered", new ILiteral[] { np1, np2, np3, p4 });
            mc.SetInitial (start, 1);
            
            mc.AddTransition (start, ntry);
            mc.AddTransition (ntry, .1d, lost);
            mc.AddTransition (lost, ntry);
            mc.AddTransition (ntry, .9d, delivered);
            mc.AddTransition (delivered, start);
            
            return mc;
        }
        
        public static MarkovChain<AutomatonNode> GetExampleFig102 ()
        {
            var mc = new MarkovChain<AutomatonNode> (new AutomatonNodeFactory ());
            var s0 = mc.AddNode ("s0");
            var s123 = mc.AddNode ("s123");
            var s456 = mc.AddNode ("s456");
            var ss123 = mc.AddNode ("s'123");
            var s23 = mc.AddNode ("s23");
            var s45 = mc.AddNode ("s45");
            var ss456 = mc.AddNode ("s'456");
            var t1 = mc.AddNode ("1");
            var t2 = mc.AddNode ("2");
            var t3 = mc.AddNode ("3");
            var t4 = mc.AddNode ("4");
            var t5 = mc.AddNode ("5");
            var t6 = mc.AddNode ("6");
            mc.SetInitial (s0, 1);

            mc.AddTransition (s0, 1d/2, s123);
            mc.AddTransition (s0, 1d/2, s456);
            
            mc.AddTransition (s123, 1d/2, ss123);
            mc.AddTransition (s123, 1d/2, s23);
            
            mc.AddTransition (ss123, 1d/2, s123);
            mc.AddTransition (ss123, 1d/2, t1);

            mc.AddTransition (s23, 1d/2, t2);
            mc.AddTransition (s23, 1d/2, t3);
            
            mc.AddTransition (s45, 1d/2, t4);
            mc.AddTransition (s45, 1d/2, t5);
            
            mc.AddTransition (s456, 1d/2, s45);
            mc.AddTransition (s456, 1d/2, ss456);
            
            mc.AddTransition (ss456, 1d/2, s456);
            mc.AddTransition (ss456, 1d/2, t6);
            
            mc.AddTransition (t1, t1);
            mc.AddTransition (t2, t2);
            mc.AddTransition (t3, t3);
            mc.AddTransition (t4, t4);
            mc.AddTransition (t5, t5);
            mc.AddTransition (t6, t6);
            
            return mc;
        }
        
        public static MarkovChain<AutomatonNode> GetExampleFig103 ()
        {
            var mc = new MarkovChain<AutomatonNode> (new AutomatonNodeFactory ());
            
            var start = mc.AddNode ("start", new Proposition ("start"));
            var s4 = mc.AddNode ("4", new Proposition ("4"));
            var s10 = mc.AddNode ("10", new Proposition ("10"));
            var s5 = mc.AddNode ("5", new Proposition ("5"));
            var s9 = mc.AddNode ("9", new Proposition ("9"));
            var s6 = mc.AddNode ("6", new Proposition ("6"));
            var s8 = mc.AddNode ("8", new Proposition ("8"));
            var won = mc.AddNode ("won", new Proposition ("won"));
            var lost = mc.AddNode ("lost", new Proposition ("lost"));
            mc.SetInitial (start, 1);
            
            mc.AddTransition (start, 2d/9, won);
            mc.AddTransition (start, 1d/12, s4);
            mc.AddTransition (start, 1d/12, s10);
            mc.AddTransition (start, 1d/9, s5);
            mc.AddTransition (start, 1d/9, s9);
            mc.AddTransition (start, 5d/36, s6);
            mc.AddTransition (start, 5d/36, s8);
            mc.AddTransition (start, 1d/9, lost);
            
            mc.AddTransition (s4, 3d/4, s4);
            mc.AddTransition (s4, 1d/12, won);
            mc.AddTransition (s4, 1d/6, lost);

            mc.AddTransition (s10, 3d/4, s10);
            mc.AddTransition (s10, 1d/12, won);
            mc.AddTransition (s10, 1d/6, lost);

            mc.AddTransition (s5, 13d/18, s5);
            mc.AddTransition (s5, 1d/9, won);
            mc.AddTransition (s5, 1d/6, lost);

            mc.AddTransition (s9, 13d/18, s9);
            mc.AddTransition (s9, 1d/9, won);
            mc.AddTransition (s9, 1d/6, lost);

            mc.AddTransition (s6, 25d/36, s6);
            mc.AddTransition (s6, 5d/36, won);
            mc.AddTransition (s6, 1d/6, lost);

            mc.AddTransition (s8, 25d/36, s8);
            mc.AddTransition (s8, 5d/36, won);
            mc.AddTransition (s8, 1d/6, lost);
            
            mc.AddTransition (won, won);
            mc.AddTransition (lost, lost);
            
            return mc;
        }
        
        public static MarkovChain<AutomatonNode> GetExamplePMCLecture1513 () {
            var mc = new MarkovChain<AutomatonNode> (new AutomatonNodeFactory ());
            var s0 = mc.AddNode ("s0");
            var s1 = mc.AddNode ("s1");
            var s2 = mc.AddNode ("s2");
            var s3 = mc.AddNode ("s3");
            var s4 = mc.AddNode ("s4");
            var s5 = mc.AddNode ("s5");
            
            mc.AddTransition (s0, .5, s1);
            mc.AddTransition (s0, .5, s3);
            
            mc.AddTransition (s1, .5, s0);
            mc.AddTransition (s1, .25, s4);
            mc.AddTransition (s1, .25, s2);
            
            mc.AddTransition (s2, s5);
            mc.AddTransition (s5, s2);
            
            mc.AddTransition (s3, s3);
            mc.AddTransition (s4, s4);
            
            mc.SetInitial (s0, 1);
            
            return mc;
        }
        
        public static MarkovChain<AutomatonNode> GetExample (string example) {
            if (example == "101") {
                return GetExampleFig101 ();
            } else if (example == "102" | example == "die") {
                return GetExampleFig102 ();
            } else if (example == "103" | example == "craps") {
                return GetExampleFig103 ();
            } else if (example == "pmc-lecture-15-13") {
                return GetExamplePMCLecture1513 ();
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
            
            Assert.That (post.All (v => expectedPost.Contains (v)), "Not all nodes were expected.");
            Assert.That (expectedPost.All (v => post.Contains (v)), "All expected nodes not contained in post.");
        }
        
        [TestCase (@"101", "start",     new [] { "start", "delivered", "lost", "try" })]
        [TestCase (@"101", "try",       new [] { "start", "delivered", "lost", "try" })]
        [TestCase (@"101", "delivered", new [] { "start", "delivered", "lost", "try" })]
        [TestCase (@"101", "lost",      new [] { "start", "delivered", "lost", "try" })]
        [TestCase (@"102", "s0",        new string[] { })]
        [TestCase (@"102", "s123",      new [] { "s0", "s'123", "s123" })]
        [TestCase (@"102", "s456",      new [] { "s0", "s'456", "s456" })]
        [TestCase (@"102", "s'123",     new [] { "s123", "s0", "s'123" })]
        [TestCase (@"102", "s23",       new [] { "s123", "s0", "s'123" })]
        [TestCase (@"102", "s45",       new [] { "s456", "s'456", "s0" })]
        [TestCase (@"102", "s'456",     new [] { "s456", "s'456", "s0" })]
        [TestCase (@"102", "1",         new [] { "1", "s'123", "s123", "s0" })]
        [TestCase (@"102", "2",         new [] { "2", "s23", "s123", "s'123", "s0" })]
        [TestCase (@"102", "3",         new [] { "3", "s23", "s123", "s'123", "s0" })]
        [TestCase (@"102", "4",         new [] { "4", "s45", "s456", "s'456", "s0" })]
        [TestCase (@"102", "5",         new [] { "5", "s45", "s456", "s'456", "s0" })]
        [TestCase (@"102", "6",         new [] { "6", "s'456", "s456", "s0" })]
        public void TestAllPred (string example, string source, string[] expected)
        {
            var mc = GetExample (example);
            var start = mc.GetVertex (source);
            
            Console.WriteLine (mc.ToDot ());

            var post = mc.AllPre (start);
            var expectedPost = expected.Select (v => mc.GetVertex (v));
            
            Assert.That (post.All (v => expectedPost.Contains (v)), "Not all nodes were expected.");
            Assert.That (expectedPost.All (v => post.Contains (v)), "All expected nodes not contained in post.\n" +
                         "Expected : {" + string.Join (",", expectedPost.Select (t => t.Name)) + "}\n" +
                         "But was : {" + string.Join (",", post.Select (t => t.Name)) + "}");
        }

        [TestCase (@"101", "start",     new [] { "delivered" })]
        [TestCase (@"101", "try",       new [] { "start", "lost" })]
        [TestCase (@"101", "delivered", new [] { "try" })]
        [TestCase (@"101", "lost",      new [] { "try" })]
        [TestCase (@"102", "s0",        new string[] { })]
        [TestCase (@"102", "s123",      new [] { "s0", "s'123" })]
        [TestCase (@"102", "s456",      new [] { "s0", "s'456" })]
        [TestCase (@"102", "s'123",     new [] { "s123" })]
        [TestCase (@"102", "s23",       new [] { "s123" })]
        [TestCase (@"102", "s45",       new [] { "s456" })]
        [TestCase (@"102", "s'456",     new [] { "s456" })]
        [TestCase (@"102", "1",         new [] { "1", "s'123" })]
        [TestCase (@"102", "2",         new [] { "2", "s23" })]
        [TestCase (@"102", "3",         new [] { "3", "s23" })]
        [TestCase (@"102", "4",         new [] { "4", "s45" })]
        [TestCase (@"102", "5",         new [] { "5", "s45" })]
        [TestCase (@"102", "6",         new [] { "6", "s'456" })]
        public void TestPred (string example, string source, string[] expected)
        {
            var mc = GetExample (example);
            var start = mc.GetVertex (source);

            var post = mc.Pre (start);
            var expectedPost = expected.Select (v => mc.GetVertex (v));

            Console.WriteLine (string.Join (",", post.Select (x => x.Name)));
            Console.WriteLine (string.Join (",", expectedPost.Select (x => x.Name)));
            
            Assert.That (post.All (v => expectedPost.Contains (v)), "Not all nodes were expected.");
            Assert.That (expectedPost.All (v => post.Contains (v)), "All expected nodes not contained in post.");
        }
        
        [TestCase (@"101", "start",     false)] 
        [TestCase (@"101", "try",       false)] 
        [TestCase (@"101", "delivered", false)] 
        [TestCase (@"101", "lost",      false)] 
        [TestCase (@"102", "s0",        false)] 
        [TestCase (@"102", "s123",      false)] 
        [TestCase (@"102", "s456",      false)] 
        [TestCase (@"102", "s'123",     false)] 
        [TestCase (@"102", "s23",       false)] 
        [TestCase (@"102", "s45",       false)] 
        [TestCase (@"102", "s'456",     false)] 
        [TestCase (@"102", "1",         true)] 
        [TestCase (@"102", "2",         true)]
        [TestCase (@"102", "3",         true)]
        [TestCase (@"102", "4",         true)]
        [TestCase (@"102", "5",         true)]
        [TestCase (@"102", "6",         true)] 
        public void TestAbsorbing (string example, string source, bool absorbing)
        {
            var mc = GetExample (example);
            var start = mc.GetVertex (source);

            Assert.AreEqual (absorbing, mc.IsAbsorbing (start));
        }
    }
}

