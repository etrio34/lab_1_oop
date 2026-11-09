grammar LabCalculator;



compileUnit : expression EOF;

expression :
      LPAREN expression RPAREN                             #ParenthesizedExpr
    | NOT expression                                       #NotExpr
    | operatorToken=(ADD | SUBTRACT) expression            #UnaryExpr
    | MMAX LPAREN expression (COMMA expression)* RPAREN    #MmaxExpr
    | MMIN LPAREN expression (COMMA expression)* RPAREN    #MminExpr
    | expression operatorToken=(MULTIPLY | DIVIDE) expression #MultiplicativeExpr
    | expression operatorToken=(ADD | SUBTRACT) expression    #AdditiveExpr
    | expression operatorToken=(EQUAL | LESS | GREATER) expression #RelationalExpr
    | NUMBER                                               #NumberExpr
    | IDENTIFIER                                           #IdentifierExpr
    ;


MMAX       : 'mmax' | 'MMAX';
MMIN       : 'mmin' | 'MMIN';
NOT        : 'not' | 'NOT';
EQUAL      : '=';
LESS       : '<';
GREATER    : '>';
COMMA      : ',';
NUMBER     : INT ('.' INT)?;
IDENTIFIER : [a-zA-Z]+[1-9][0-9]*;
INT        : ('0'..'9')+;
MULTIPLY   : '*';
DIVIDE     : '/';
SUBTRACT   : '-';
ADD        : '+';
LPAREN     : '(';
RPAREN     : ')';
WS         : [ \t\r\n] -> channel(HIDDEN);