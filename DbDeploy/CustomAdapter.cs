using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace OmniDbDeploy
{
 public class CustomAdapter : System.Data.Common.DbDataAdapter 
 { 
  public int FillFromReader(DataTable dataTable, IDataReader dataReader)
  { 
   return this.Fill( dataTable, dataReader ); 
  } 
  protected override System.Data.Common.RowUpdatedEventArgs CreateRowUpdatedEvent( DataRow a, IDbCommand b, StatementType c, System.Data.Common.DataTableMapping d )
  {
   return ( System.Data.Common.RowUpdatedEventArgs )new EventArgs();
  }
    
  protected override System.Data.Common.RowUpdatingEventArgs CreateRowUpdatingEvent( DataRow a, IDbCommand b, StatementType c, System.Data.Common.DataTableMapping d )
  {
   return ( System.Data.Common.RowUpdatingEventArgs )new EventArgs();
  }
  
  protected override void OnRowUpdated( System.Data.Common.RowUpdatedEventArgs value )
  {
  }
  protected override void OnRowUpdating( System.Data.Common.RowUpdatingEventArgs value )
  {
  } 
 } 
}
