grammar LTL; 

options {
	language = 'CSharp3'; 
	output=AST; 
} 

@parser::namespace { LtlSharp }
@lexer::namespace  { LtlSharp }

public parse returns [LTLFormula value]
  :  formula EOF {$value = $formula.value;}
  ;
  
formula returns [LTLFormula value]
  :  f = equivalence {$value = $f.value;}
  ;

equivalence returns [LTLFormula value]
  :  a = binary { $value = $a.value; } 
     ('<->' b = equivalence { $value = new Equivalence ($a.value, $b.value); })?
  ;

binary returns [LTLFormula value]
  :  a = implication { $value = $a.value; } 
     ( 'U' b = binary { $value = new Until ($a.value, $b.value); }
     | 'R' b = binary { $value = new Release ($a.value, $b.value); }
     | 'W' b = binary { $value = new Unless ($a.value, $b.value); }
     )?
  ;
    
implication returns [LTLFormula value]
  :  a = conjunction { $value = $a.value; } 
     ('->' b = implication { $value = new Implication ($a.value, $b.value); })?
  ;
  
conjunction returns [LTLFormula value]
  :  a = disjunction { $value = $a.value; } 
     ('&' b = conjunction { if ($value.GetType() == typeof(Conjunction)) { ((Conjunction) $value).Push ($b.value); } else { $value = new Conjunction ($value, $b.value); } })?
  ;
  
disjunction returns [LTLFormula value]
  :  a = unary { $value = $a.value; } 
     ('|' b = disjunction { if ($value.GetType() == typeof(Disjunction)) { ((Disjunction) $value).Push ($b.value); } else { $value = new Disjunction ($value, $b.value); } })?
  ;

unary returns [LTLFormula value]
  :  atom { $value = $atom.value; }
     | '!' a = unary { $value = new Negation ($a.value); }
     | 'X' a = unary { $value = new Next ($a.value); }
     | 'F' a = unary { $value = new Finally ($a.value); }
     | 'G' a = unary { $value = new Globally ($a.value); }
  ;

atom returns [LTLFormula value]
  :  PROPOSITION { $value = new Proposition ($PROPOSITION.Text); }
     | '(' formula ')' { $value = new ParenthesedExpression ($formula.value); }
  ;

PROPOSITION : ('a'..'z') ('a'..'z'|'A'..'Z'|'_'|'0'..'9')* ;