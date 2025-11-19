using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountLib;

//tranzactie catre utilizator
//record: immutable by default, 
public record TransactionInfo(
   decimal Amount,
   DateTime Date,
   TransactionType Type
);
