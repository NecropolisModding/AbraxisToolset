using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbraxisToolset.CSVFiles {
    public static class ATCSVUtil {
        public static string GetPrefix(string value) {
            string prefix = "";
            int index = 0;

            for( int i = 0; i < value.Length; i++ ) {
                if( value[i] == '_' )
                    break;
                index++;
            }

            if( value.Length == index ) {
                return prefix;
            }

            prefix = value.Substring( 0, index );

            return prefix;
        }

        public static string GetWithoutPrefix(string value) {
            int index = 0;

            for( int i = 0; i < value.Length; i++ ) {
                if( value[i] == '_' )
                    break;
                index++;
            }

            if( value.Length == index ) {
                return value;
            }
            index++;
            return value.Substring( index, value.Length - index );
        }
    }
}
