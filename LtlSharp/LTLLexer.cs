//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 3.4
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// $ANTLR 3.4 ./LtlSharp/LTL.g 2012-07-16 10:48:00

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 219
// Unreachable code detected.
#pragma warning disable 162
// Missing XML comment for publicly visible type or member 'Type_or_Member'
#pragma warning disable 1591
// CLS compliance checking will not be performed on 'type' because it is not visible from outside this assembly.
#pragma warning disable 3019


using System.Collections.Generic;
using Antlr.Runtime;
using Antlr.Runtime.Misc;

namespace  LtlSharp 
{
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.4")]
[System.CLSCompliant(false)]
public partial class LTLLexer : Antlr.Runtime.Lexer
{
	public const int EOF=-1;
	public const int T__5=5;
	public const int T__6=6;
	public const int T__7=7;
	public const int T__8=8;
	public const int T__9=9;
	public const int T__10=10;
	public const int T__11=11;
	public const int T__12=12;
	public const int T__13=13;
	public const int T__14=14;
	public const int T__15=15;
	public const int T__16=16;
	public const int T__17=17;
	public const int T__18=18;
	public const int T__19=19;
	public const int PROPOSITION=4;

    // delegates
    // delegators

	public LTLLexer()
	{
		OnCreated();
	}

	public LTLLexer(ICharStream input )
		: this(input, new RecognizerSharedState())
	{
	}

	public LTLLexer(ICharStream input, RecognizerSharedState state)
		: base(input, state)
	{

		OnCreated();
	}
	public override string GrammarFileName { get { return "./LtlSharp/LTL.g"; } }


	partial void OnCreated();
	partial void EnterRule(string ruleName, int ruleIndex);
	partial void LeaveRule(string ruleName, int ruleIndex);

	partial void EnterRule_T__5();
	partial void LeaveRule_T__5();

	// $ANTLR start "T__5"
	[GrammarRule("T__5")]
	private void mT__5()
	{
		EnterRule_T__5();
		EnterRule("T__5", 1);
		TraceIn("T__5", 1);
		try
		{
			int _type = T__5;
			int _channel = DefaultTokenChannel;
			// ./LtlSharp/LTL.g:9:6: ( '!' )
			DebugEnterAlt(1);
			// ./LtlSharp/LTL.g:9:8: '!'
			{
			DebugLocation(9, 8);
			Match('!'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__5", 1);
			LeaveRule("T__5", 1);
			LeaveRule_T__5();
		}
	}
	// $ANTLR end "T__5"

	partial void EnterRule_T__6();
	partial void LeaveRule_T__6();

	// $ANTLR start "T__6"
	[GrammarRule("T__6")]
	private void mT__6()
	{
		EnterRule_T__6();
		EnterRule("T__6", 2);
		TraceIn("T__6", 2);
		try
		{
			int _type = T__6;
			int _channel = DefaultTokenChannel;
			// ./LtlSharp/LTL.g:10:6: ( '&' )
			DebugEnterAlt(1);
			// ./LtlSharp/LTL.g:10:8: '&'
			{
			DebugLocation(10, 8);
			Match('&'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__6", 2);
			LeaveRule("T__6", 2);
			LeaveRule_T__6();
		}
	}
	// $ANTLR end "T__6"

	partial void EnterRule_T__7();
	partial void LeaveRule_T__7();

	// $ANTLR start "T__7"
	[GrammarRule("T__7")]
	private void mT__7()
	{
		EnterRule_T__7();
		EnterRule("T__7", 3);
		TraceIn("T__7", 3);
		try
		{
			int _type = T__7;
			int _channel = DefaultTokenChannel;
			// ./LtlSharp/LTL.g:11:6: ( '(' )
			DebugEnterAlt(1);
			// ./LtlSharp/LTL.g:11:8: '('
			{
			DebugLocation(11, 8);
			Match('('); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__7", 3);
			LeaveRule("T__7", 3);
			LeaveRule_T__7();
		}
	}
	// $ANTLR end "T__7"

	partial void EnterRule_T__8();
	partial void LeaveRule_T__8();

	// $ANTLR start "T__8"
	[GrammarRule("T__8")]
	private void mT__8()
	{
		EnterRule_T__8();
		EnterRule("T__8", 4);
		TraceIn("T__8", 4);
		try
		{
			int _type = T__8;
			int _channel = DefaultTokenChannel;
			// ./LtlSharp/LTL.g:12:6: ( ')' )
			DebugEnterAlt(1);
			// ./LtlSharp/LTL.g:12:8: ')'
			{
			DebugLocation(12, 8);
			Match(')'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__8", 4);
			LeaveRule("T__8", 4);
			LeaveRule_T__8();
		}
	}
	// $ANTLR end "T__8"

	partial void EnterRule_T__9();
	partial void LeaveRule_T__9();

	// $ANTLR start "T__9"
	[GrammarRule("T__9")]
	private void mT__9()
	{
		EnterRule_T__9();
		EnterRule("T__9", 5);
		TraceIn("T__9", 5);
		try
		{
			int _type = T__9;
			int _channel = DefaultTokenChannel;
			// ./LtlSharp/LTL.g:13:6: ( '->' )
			DebugEnterAlt(1);
			// ./LtlSharp/LTL.g:13:8: '->'
			{
			DebugLocation(13, 8);
			Match("->"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__9", 5);
			LeaveRule("T__9", 5);
			LeaveRule_T__9();
		}
	}
	// $ANTLR end "T__9"

	partial void EnterRule_T__10();
	partial void LeaveRule_T__10();

	// $ANTLR start "T__10"
	[GrammarRule("T__10")]
	private void mT__10()
	{
		EnterRule_T__10();
		EnterRule("T__10", 6);
		TraceIn("T__10", 6);
		try
		{
			int _type = T__10;
			int _channel = DefaultTokenChannel;
			// ./LtlSharp/LTL.g:14:7: ( '<->' )
			DebugEnterAlt(1);
			// ./LtlSharp/LTL.g:14:9: '<->'
			{
			DebugLocation(14, 9);
			Match("<->"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__10", 6);
			LeaveRule("T__10", 6);
			LeaveRule_T__10();
		}
	}
	// $ANTLR end "T__10"

	partial void EnterRule_T__11();
	partial void LeaveRule_T__11();

	// $ANTLR start "T__11"
	[GrammarRule("T__11")]
	private void mT__11()
	{
		EnterRule_T__11();
		EnterRule("T__11", 7);
		TraceIn("T__11", 7);
		try
		{
			int _type = T__11;
			int _channel = DefaultTokenChannel;
			// ./LtlSharp/LTL.g:15:7: ( '<=>' )
			DebugEnterAlt(1);
			// ./LtlSharp/LTL.g:15:9: '<=>'
			{
			DebugLocation(15, 9);
			Match("<=>"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__11", 7);
			LeaveRule("T__11", 7);
			LeaveRule_T__11();
		}
	}
	// $ANTLR end "T__11"

	partial void EnterRule_T__12();
	partial void LeaveRule_T__12();

	// $ANTLR start "T__12"
	[GrammarRule("T__12")]
	private void mT__12()
	{
		EnterRule_T__12();
		EnterRule("T__12", 8);
		TraceIn("T__12", 8);
		try
		{
			int _type = T__12;
			int _channel = DefaultTokenChannel;
			// ./LtlSharp/LTL.g:16:7: ( '=>' )
			DebugEnterAlt(1);
			// ./LtlSharp/LTL.g:16:9: '=>'
			{
			DebugLocation(16, 9);
			Match("=>"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__12", 8);
			LeaveRule("T__12", 8);
			LeaveRule_T__12();
		}
	}
	// $ANTLR end "T__12"

	partial void EnterRule_T__13();
	partial void LeaveRule_T__13();

	// $ANTLR start "T__13"
	[GrammarRule("T__13")]
	private void mT__13()
	{
		EnterRule_T__13();
		EnterRule("T__13", 9);
		TraceIn("T__13", 9);
		try
		{
			int _type = T__13;
			int _channel = DefaultTokenChannel;
			// ./LtlSharp/LTL.g:17:7: ( 'F' )
			DebugEnterAlt(1);
			// ./LtlSharp/LTL.g:17:9: 'F'
			{
			DebugLocation(17, 9);
			Match('F'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__13", 9);
			LeaveRule("T__13", 9);
			LeaveRule_T__13();
		}
	}
	// $ANTLR end "T__13"

	partial void EnterRule_T__14();
	partial void LeaveRule_T__14();

	// $ANTLR start "T__14"
	[GrammarRule("T__14")]
	private void mT__14()
	{
		EnterRule_T__14();
		EnterRule("T__14", 10);
		TraceIn("T__14", 10);
		try
		{
			int _type = T__14;
			int _channel = DefaultTokenChannel;
			// ./LtlSharp/LTL.g:18:7: ( 'G' )
			DebugEnterAlt(1);
			// ./LtlSharp/LTL.g:18:9: 'G'
			{
			DebugLocation(18, 9);
			Match('G'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__14", 10);
			LeaveRule("T__14", 10);
			LeaveRule_T__14();
		}
	}
	// $ANTLR end "T__14"

	partial void EnterRule_T__15();
	partial void LeaveRule_T__15();

	// $ANTLR start "T__15"
	[GrammarRule("T__15")]
	private void mT__15()
	{
		EnterRule_T__15();
		EnterRule("T__15", 11);
		TraceIn("T__15", 11);
		try
		{
			int _type = T__15;
			int _channel = DefaultTokenChannel;
			// ./LtlSharp/LTL.g:19:7: ( 'R' )
			DebugEnterAlt(1);
			// ./LtlSharp/LTL.g:19:9: 'R'
			{
			DebugLocation(19, 9);
			Match('R'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__15", 11);
			LeaveRule("T__15", 11);
			LeaveRule_T__15();
		}
	}
	// $ANTLR end "T__15"

	partial void EnterRule_T__16();
	partial void LeaveRule_T__16();

	// $ANTLR start "T__16"
	[GrammarRule("T__16")]
	private void mT__16()
	{
		EnterRule_T__16();
		EnterRule("T__16", 12);
		TraceIn("T__16", 12);
		try
		{
			int _type = T__16;
			int _channel = DefaultTokenChannel;
			// ./LtlSharp/LTL.g:20:7: ( 'U' )
			DebugEnterAlt(1);
			// ./LtlSharp/LTL.g:20:9: 'U'
			{
			DebugLocation(20, 9);
			Match('U'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__16", 12);
			LeaveRule("T__16", 12);
			LeaveRule_T__16();
		}
	}
	// $ANTLR end "T__16"

	partial void EnterRule_T__17();
	partial void LeaveRule_T__17();

	// $ANTLR start "T__17"
	[GrammarRule("T__17")]
	private void mT__17()
	{
		EnterRule_T__17();
		EnterRule("T__17", 13);
		TraceIn("T__17", 13);
		try
		{
			int _type = T__17;
			int _channel = DefaultTokenChannel;
			// ./LtlSharp/LTL.g:21:7: ( 'W' )
			DebugEnterAlt(1);
			// ./LtlSharp/LTL.g:21:9: 'W'
			{
			DebugLocation(21, 9);
			Match('W'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__17", 13);
			LeaveRule("T__17", 13);
			LeaveRule_T__17();
		}
	}
	// $ANTLR end "T__17"

	partial void EnterRule_T__18();
	partial void LeaveRule_T__18();

	// $ANTLR start "T__18"
	[GrammarRule("T__18")]
	private void mT__18()
	{
		EnterRule_T__18();
		EnterRule("T__18", 14);
		TraceIn("T__18", 14);
		try
		{
			int _type = T__18;
			int _channel = DefaultTokenChannel;
			// ./LtlSharp/LTL.g:22:7: ( 'X' )
			DebugEnterAlt(1);
			// ./LtlSharp/LTL.g:22:9: 'X'
			{
			DebugLocation(22, 9);
			Match('X'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__18", 14);
			LeaveRule("T__18", 14);
			LeaveRule_T__18();
		}
	}
	// $ANTLR end "T__18"

	partial void EnterRule_T__19();
	partial void LeaveRule_T__19();

	// $ANTLR start "T__19"
	[GrammarRule("T__19")]
	private void mT__19()
	{
		EnterRule_T__19();
		EnterRule("T__19", 15);
		TraceIn("T__19", 15);
		try
		{
			int _type = T__19;
			int _channel = DefaultTokenChannel;
			// ./LtlSharp/LTL.g:23:7: ( '|' )
			DebugEnterAlt(1);
			// ./LtlSharp/LTL.g:23:9: '|'
			{
			DebugLocation(23, 9);
			Match('|'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__19", 15);
			LeaveRule("T__19", 15);
			LeaveRule_T__19();
		}
	}
	// $ANTLR end "T__19"

	partial void EnterRule_PROPOSITION();
	partial void LeaveRule_PROPOSITION();

	// $ANTLR start "PROPOSITION"
	[GrammarRule("PROPOSITION")]
	private void mPROPOSITION()
	{
		EnterRule_PROPOSITION();
		EnterRule("PROPOSITION", 16);
		TraceIn("PROPOSITION", 16);
		try
		{
			int _type = PROPOSITION;
			int _channel = DefaultTokenChannel;
			// ./LtlSharp/LTL.g:66:13: ( ( 'a' .. 'z' ) ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )* )
			DebugEnterAlt(1);
			// ./LtlSharp/LTL.g:66:15: ( 'a' .. 'z' ) ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )*
			{
			DebugLocation(66, 15);
			if ((input.LA(1)>='a' && input.LA(1)<='z'))
			{
				input.Consume();
			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;
			}

			DebugLocation(66, 26);
			// ./LtlSharp/LTL.g:66:26: ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )*
			try { DebugEnterSubRule(1);
			while (true)
			{
				int alt1=2;
				try { DebugEnterDecision(1, false);
				int LA1_0 = input.LA(1);

				if (((LA1_0>='0' && LA1_0<='9')||(LA1_0>='A' && LA1_0<='Z')||LA1_0=='_'||(LA1_0>='a' && LA1_0<='z')))
				{
					alt1 = 1;
				}


				} finally { DebugExitDecision(1); }
				switch ( alt1 )
				{
				case 1:
					DebugEnterAlt(1);
					// ./LtlSharp/LTL.g:
					{
					DebugLocation(66, 26);
					input.Consume();


					}
					break;

				default:
					goto loop1;
				}
			}

			loop1:
				;

			} finally { DebugExitSubRule(1); }


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("PROPOSITION", 16);
			LeaveRule("PROPOSITION", 16);
			LeaveRule_PROPOSITION();
		}
	}
	// $ANTLR end "PROPOSITION"

	public override void mTokens()
	{
		// ./LtlSharp/LTL.g:1:8: ( T__5 | T__6 | T__7 | T__8 | T__9 | T__10 | T__11 | T__12 | T__13 | T__14 | T__15 | T__16 | T__17 | T__18 | T__19 | PROPOSITION )
		int alt2=16;
		try { DebugEnterDecision(2, false);
		switch (input.LA(1))
		{
		case '!':
			{
			alt2 = 1;
			}
			break;
		case '&':
			{
			alt2 = 2;
			}
			break;
		case '(':
			{
			alt2 = 3;
			}
			break;
		case ')':
			{
			alt2 = 4;
			}
			break;
		case '-':
			{
			alt2 = 5;
			}
			break;
		case '<':
			{
			int LA2_6 = input.LA(2);

			if ((LA2_6=='-'))
			{
				alt2 = 6;
			}
			else if ((LA2_6=='='))
			{
				alt2 = 7;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 2, 6, input);
				DebugRecognitionException(nvae);
				throw nvae;
			}
			}
			break;
		case '=':
			{
			alt2 = 8;
			}
			break;
		case 'F':
			{
			alt2 = 9;
			}
			break;
		case 'G':
			{
			alt2 = 10;
			}
			break;
		case 'R':
			{
			alt2 = 11;
			}
			break;
		case 'U':
			{
			alt2 = 12;
			}
			break;
		case 'W':
			{
			alt2 = 13;
			}
			break;
		case 'X':
			{
			alt2 = 14;
			}
			break;
		case '|':
			{
			alt2 = 15;
			}
			break;
		case 'a':
		case 'b':
		case 'c':
		case 'd':
		case 'e':
		case 'f':
		case 'g':
		case 'h':
		case 'i':
		case 'j':
		case 'k':
		case 'l':
		case 'm':
		case 'n':
		case 'o':
		case 'p':
		case 'q':
		case 'r':
		case 's':
		case 't':
		case 'u':
		case 'v':
		case 'w':
		case 'x':
		case 'y':
		case 'z':
			{
			alt2 = 16;
			}
			break;
		default:
			{
				NoViableAltException nvae = new NoViableAltException("", 2, 0, input);
				DebugRecognitionException(nvae);
				throw nvae;
			}
		}

		} finally { DebugExitDecision(2); }
		switch (alt2)
		{
		case 1:
			DebugEnterAlt(1);
			// ./LtlSharp/LTL.g:1:10: T__5
			{
			DebugLocation(1, 10);
			mT__5(); 

			}
			break;
		case 2:
			DebugEnterAlt(2);
			// ./LtlSharp/LTL.g:1:15: T__6
			{
			DebugLocation(1, 15);
			mT__6(); 

			}
			break;
		case 3:
			DebugEnterAlt(3);
			// ./LtlSharp/LTL.g:1:20: T__7
			{
			DebugLocation(1, 20);
			mT__7(); 

			}
			break;
		case 4:
			DebugEnterAlt(4);
			// ./LtlSharp/LTL.g:1:25: T__8
			{
			DebugLocation(1, 25);
			mT__8(); 

			}
			break;
		case 5:
			DebugEnterAlt(5);
			// ./LtlSharp/LTL.g:1:30: T__9
			{
			DebugLocation(1, 30);
			mT__9(); 

			}
			break;
		case 6:
			DebugEnterAlt(6);
			// ./LtlSharp/LTL.g:1:35: T__10
			{
			DebugLocation(1, 35);
			mT__10(); 

			}
			break;
		case 7:
			DebugEnterAlt(7);
			// ./LtlSharp/LTL.g:1:41: T__11
			{
			DebugLocation(1, 41);
			mT__11(); 

			}
			break;
		case 8:
			DebugEnterAlt(8);
			// ./LtlSharp/LTL.g:1:47: T__12
			{
			DebugLocation(1, 47);
			mT__12(); 

			}
			break;
		case 9:
			DebugEnterAlt(9);
			// ./LtlSharp/LTL.g:1:53: T__13
			{
			DebugLocation(1, 53);
			mT__13(); 

			}
			break;
		case 10:
			DebugEnterAlt(10);
			// ./LtlSharp/LTL.g:1:59: T__14
			{
			DebugLocation(1, 59);
			mT__14(); 

			}
			break;
		case 11:
			DebugEnterAlt(11);
			// ./LtlSharp/LTL.g:1:65: T__15
			{
			DebugLocation(1, 65);
			mT__15(); 

			}
			break;
		case 12:
			DebugEnterAlt(12);
			// ./LtlSharp/LTL.g:1:71: T__16
			{
			DebugLocation(1, 71);
			mT__16(); 

			}
			break;
		case 13:
			DebugEnterAlt(13);
			// ./LtlSharp/LTL.g:1:77: T__17
			{
			DebugLocation(1, 77);
			mT__17(); 

			}
			break;
		case 14:
			DebugEnterAlt(14);
			// ./LtlSharp/LTL.g:1:83: T__18
			{
			DebugLocation(1, 83);
			mT__18(); 

			}
			break;
		case 15:
			DebugEnterAlt(15);
			// ./LtlSharp/LTL.g:1:89: T__19
			{
			DebugLocation(1, 89);
			mT__19(); 

			}
			break;
		case 16:
			DebugEnterAlt(16);
			// ./LtlSharp/LTL.g:1:95: PROPOSITION
			{
			DebugLocation(1, 95);
			mPROPOSITION(); 

			}
			break;

		}

	}


	#region DFA

	protected override void InitDFAs()
	{
		base.InitDFAs();
	}

 
	#endregion

}

} // namespace  LtlSharp 
