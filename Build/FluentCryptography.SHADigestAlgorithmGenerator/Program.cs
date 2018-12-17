/*
 * Copyright 2018 Stanislav Muhametsin. All rights Reserved.
 *
 * Licensed  under the  Apache License,  Version 2.0  (the "License");
 * you may not use  this file  except in  compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed  under the  License is distributed on an "AS IS" BASIS,
 * WITHOUT  WARRANTIES OR CONDITIONS  OF ANY KIND, either  express  or
 * implied.
 *
 * See the License for the specific language governing permissions and
 * limitations under the License. 
 */
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentCryptography.SHADigestAlgorithmGenerator
{
   class Program
   {
      private static readonly Encoding Encoding = new UTF8Encoding( false, true );

      // Takes one argument, which is the directory where to output files
      static async Task Main(
         String[] args
         )
      {
         var folder = args[0];
         await Task.WhenAll( new[]
         {
            SHAAlgorithms.MD5,
            SHAAlgorithms.SHA128,
            SHAAlgorithms.SHA256,
            SHAAlgorithms.SHA384,
            SHAAlgorithms.SHA512
         }.Select( algo => GenerateAlgorithm( folder, algo ) )
         );
      }

      private static async Task GenerateAlgorithm(
         String folder,
         AlgorithmGenerator generator
         )
      {
         var code = generator.GenerateAlgorithm( true );
         using ( var fs = File.Open( Path.Combine( Path.GetFullPath( folder ), generator.AlgorithmName + ".cs" ), FileMode.Create, FileAccess.Write, FileShare.None ) )
         using ( var ts = new StreamWriter( fs, Encoding, 0x1000, true ) )
         {
            await ts.WriteAsync( code );
         }
      }
   }
}
