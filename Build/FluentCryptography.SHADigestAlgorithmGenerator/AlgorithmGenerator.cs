﻿/*
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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using UtilPack;

namespace FluentCryptography.SHADigestAlgorithmGenerator
{
   public interface AlgorithmGenerator
   {
      String AlgorithmName { get; }

      String GenerateAlgorithm(
         Boolean useUnsafe
         );
   }

   public interface SHAAlgorithmGenerationStateAPI
   {
      String AlgorithmName { get; }
      Int32 DigestByteCount { get; }
      Int32 BlockSize { get; }
      Int32 ExpandedSize { get; }
      String StateTypeName { get; } // This and size can be just turned to "TypeInfo<T>" and then to get size, use UtilPack.SizeOf<T>()! would cause type parameter to be added to SHAAlgorithmGenerator tho.
      Int32 StateTypeSize { get; }
      Int32 CountIncreaseForHashEnd { get; }

      // Const data (no immutable collect
      ImmutableArray<UInt64> InitialState { get; }

      // Method names
      String LengthWriteMethodName { get; }
      String DigestWriteMethodName { get; }
      String ExpandedStateReadMethodName { get; }

      // Code
      String ExpandCode { get; }
      String TransformCodeUnsafe { get; }
      String StaticCode { get; }
      String StaticCodeOnlyUnsafe { get; }
   }

   public sealed class SHAAlgorithmGenerationStateBuilder : SHAAlgorithmGenerationStateAPI
   {
      public String AlgorithmName { get; set; }
      public Int32 DigestByteCount { get; set; }
      public Int32 BlockSize { get; set; }
      public Int32 ExpandedSize { get; set; }
      public String StateTypeName { get; set; }
      public Int32 StateTypeSize { get; set; }
      public Int32 CountIncreaseForHashEnd { get; set; }

      // Const data (no immutable collect
      public ImmutableArray<UInt64> InitialState { get; set; }

      // Method names
      public String LengthWriteMethodName { get; set; }
      public String DigestWriteMethodName { get; set; }
      public String ExpandedStateReadMethodName { get; set; }

      // Code
      public String ExpandCode { get; set; }
      public String TransformCodeUnsafe { get; set; }
      public String StaticCode { get; set; }
      public String StaticCodeOnlyUnsafe { get; set; }
   }

   public struct SHAAlgorithmGenerationState : SHAAlgorithmGenerationStateAPI
   {
      public SHAAlgorithmGenerationState(
         SHAAlgorithmGenerationStateAPI other
         ) : this(
            other.AlgorithmName,
            other.DigestByteCount,
            other.BlockSize,
            other.ExpandedSize,
            other.StateTypeName,
            other.StateTypeSize,
            other.CountIncreaseForHashEnd,
            other.InitialState,
            other.LengthWriteMethodName,
            other.DigestWriteMethodName,
            other.ExpandedStateReadMethodName,
            other.ExpandCode,
            other.TransformCodeUnsafe,
            other.StaticCode,
            other.StaticCodeOnlyUnsafe
            )
      {

      }

      public SHAAlgorithmGenerationState(
         String algorithmName,
         Int32 digestByteCount,
         Int32 blockSize,
         Int32 expandedSize,
         String stateTypeName,
         Int32 stateTypeSize,
         Int32 countIncreaseForHashEnd,

         ImmutableArray<UInt64> initialState,

         String lengthWriteMethodName,
         String digestWriteMethodName,
         String expandedStateReadMethodName,

         String expandCode,
         String transformCodeUnsafe,
         String staticCode,
         String staticCodeOnlyUnsafe
         )
      {
         this.AlgorithmName = ArgumentValidator.ValidateNotEmpty( nameof( algorithmName ), algorithmName );
         this.DigestByteCount = digestByteCount;
         this.BlockSize = blockSize;
         this.ExpandedSize = expandedSize;
         this.StateTypeName = ArgumentValidator.ValidateNotEmpty( nameof( stateTypeName ), stateTypeName );
         this.StateTypeSize = stateTypeSize;
         this.CountIncreaseForHashEnd = countIncreaseForHashEnd;

         this.InitialState = initialState;

         this.LengthWriteMethodName = ArgumentValidator.ValidateNotEmpty( nameof( lengthWriteMethodName ), lengthWriteMethodName );
         this.DigestWriteMethodName = ArgumentValidator.ValidateNotEmpty( nameof( digestWriteMethodName ), digestWriteMethodName );
         this.ExpandedStateReadMethodName = ArgumentValidator.ValidateNotEmpty( nameof( expandedStateReadMethodName ), expandedStateReadMethodName );

         this.ExpandCode = expandCode;
         this.TransformCodeUnsafe = ArgumentValidator.ValidateNotEmpty( nameof( transformCodeUnsafe ), transformCodeUnsafe );
         this.StaticCode = staticCode;
         this.StaticCodeOnlyUnsafe = staticCodeOnlyUnsafe;
      }

      public String AlgorithmName { get; }
      public Int32 DigestByteCount { get; }
      public Int32 BlockSize { get; }
      public Int32 ExpandedSize { get; }
      public String StateTypeName { get; }
      public Int32 StateTypeSize { get; }
      public Int32 CountIncreaseForHashEnd { get; }

      // Const data (no immutable collect
      public ImmutableArray<UInt64> InitialState { get; }

      // Method names
      public String LengthWriteMethodName { get; }
      public String DigestWriteMethodName { get; }
      public String ExpandedStateReadMethodName { get; }

      // Code
      public String ExpandCode { get; }
      public String TransformCodeUnsafe { get; }
      public String StaticCode { get; }
      public String StaticCodeOnlyUnsafe { get; }

   }

   public sealed class SHAAlgorithmGenerator : AlgorithmGenerator
   {
      private readonly SHAAlgorithmGenerationState _generationState;

      public SHAAlgorithmGenerator( SHAAlgorithmGenerationState state )
      {
         this._generationState = state;
      }

      public SHAAlgorithmGenerator( SHAAlgorithmGenerationStateBuilder builder )
         : this( new SHAAlgorithmGenerationState( builder ) )
      {

      }

      public String AlgorithmName => this._generationState.AlgorithmName;

      public String GenerateAlgorithm(
         Boolean useUnsafe
         )
      {
         var generationState = this._generationState;
         var unsafeString = useUnsafe ? "unsafe" : "";
         var stateTypeName = generationState.StateTypeName;
         var algorithmName = generationState.AlgorithmName;
         var digestByteCount = generationState.DigestByteCount;
         var blockSize = generationState.BlockSize;
         var initialState = generationState.InitialState;
         var expandedSize = generationState.ExpandedSize;

         return $@"
using System;
using UtilPack;

namespace FluentCryptography.Digest
{{
   /// <summary>
   /// This class implements { algorithmName } digest algorithm.
   /// </summary>
   /// <remarks>
   /// This implementation is fully managed, and uses unsafe code.
   /// </remarks>
   public sealed class { algorithmName } : AbstractDisposable, BlockDigestAlgorithm
   {{
      private const Int32 BLOCK_SIZE = { blockSize };
      /// <summary>
      /// This constant contains the value indicating how many bytes is the size of digests produced by this algorithm.
      /// For { algorithmName }, it is <c>{ digestByteCount }</c> bytes.
      /// </summary>
      public const Int32 DIGEST_BYTE_COUNT = { digestByteCount };
      private const Int32 STATE_SIZE = { initialState.Length };
      private const Int32 EXPANDED_STATE_SIZE = { expandedSize };

      private Boolean _stateResetDone;
      private readonly Byte[] _block;
      private UInt64 _count;

      private readonly { stateTypeName }[] _state;
      private readonly { stateTypeName }[] _expandedState;

      /// <summary>
      /// Creates a new instance of <see cref={ "\"" + algorithmName + "\"" }/>.
      /// </summary>
      public { algorithmName }()
      {{
         this._block = new Byte[BLOCK_SIZE];
         this._state = new { stateTypeName }[STATE_SIZE];
         this._expandedState = new { stateTypeName }[EXPANDED_STATE_SIZE];
      }}

      /// <inheritdoc />
      public void ProcessBlock( Byte[] data, Int32 offset, Int32 count )
      {{
         if ( !this._stateResetDone )
         {{
            this.ResetState( false );
            this._stateResetDone = true;
         }}

         this.HashBlock( data ?? throw new ArgumentNullException( nameof( data ) ), offset, count );
      }}

      /// <inheritdoc />
      public void WriteDigest( Byte[] array, Int32 offset )
      {{
         this.HashEnd( array ?? throw new ArgumentNullException( nameof( array ) ), offset );
         this.Reset();
      }}

      /// <inheritdoc />
      public Int32 DigestByteCount => DIGEST_BYTE_COUNT;

      /// <inheritdoc />
      public void Reset()
      {{
         this._block.Clear();
         this._count = 0UL;
         this.ResetState( true );
         this._stateResetDone = false;
      }}

      /// <inheritdoc />
      public Int32 BlockSize => BLOCK_SIZE;

      /// <inheritdoc />
      protected override void Dispose( Boolean disposing )
      {{
         if ( disposing )
         {{
            this.Reset();
         }}
      }}

      private { unsafeString } void HashBlock( Byte[] data, Int32 offset, Int32 count )
      {{
         if (count > 0)
         {{" + ( useUnsafe ? $@"
            fixed (Byte* block = this._block )
            {{
               fixed ( { stateTypeName }* state = this._state )
               {{
                  fixed ( { stateTypeName }* x = this._expandedState )
                  {{" : $@"
            var block = this._block;
            var state = this._state;
            var x = this._expandedState;" ) + $@"
                     var blockOffset = (Int32) ( this._count & ( BLOCK_SIZE - 1 ) );
                     this._count += (UInt32) count;
                     
                     // 1. Transform the previous data first
                     if (blockOffset > 0 && blockOffset + count >= BLOCK_SIZE )
                     {{
                        var remainder = BLOCK_SIZE - blockOffset;
                        Buffer.BlockCopy( data, offset, this._block, blockOffset, remainder );
                        offset += remainder;
                        count -= remainder;
                        blockOffset = 0;
                        this.DoTransform( block, state, x );
                     }}

                     // 2. Transform block at a time
                     while ( count >= BLOCK_SIZE )
                     {{
                        Buffer.BlockCopy( data, offset, this._block, 0, BLOCK_SIZE );
                        offset += BLOCK_SIZE;
                        count -= BLOCK_SIZE;
                        this.DoTransform( block, state, x );
                     }}

                     // 3. Copy in the remaining data
                     if ( count > 0 )
                     {{
                        Buffer.BlockCopy( data, offset, this._block, blockOffset, count );
                     }}" + ( useUnsafe ? $@"
                  }}
               }}
            }}" : "" ) + $@"
         }}
      }}

      private void HashEnd( Byte[] array, Int32 offset )
      {{
         const Int32 COUNT_INCREASE = { generationState.CountIncreaseForHashEnd };
         // We will write X more bytes.
         // Round up by block size
         var count = this._count;
         var dataLen = (Int32) ( ( count + COUNT_INCREASE ).RoundUpU64( BLOCK_SIZE )
            -
            count );
         var instance = AlgorithmUtility.ArrayPool.TakeInstance();
         ResizableArray<Byte> resizableArray;
         Byte[] data;
         try
         {{
            if ( instance == null )
            {{
               instance = new ResizableArrayInstanceInfo( ( resizableArray = new ResizableArray<Byte>( initialSize: BLOCK_SIZE + COUNT_INCREASE ) ) );
               data = resizableArray.Array;
            }}
            else
            {{
               ( resizableArray = instance.Array ).CurrentMaxCapacity = dataLen;
               ( data = resizableArray.Array ).Clear( 0, dataLen );
            }}

            // Write value 128 at the beginning, and amount of written *bits* at the end of the data
            var idx = 0;
            data.WriteByteToBytes( ref idx, 0x80 );
            data.{ generationState.LengthWriteMethodName }(dataLen - 8, unchecked((Int64) count * 8) );

            // Hash the data
            this.HashBlock( data, 0, dataLen );

            // Transform state integers into byte array
            for ( var i = 0; i < DIGEST_BYTE_COUNT / sizeof( { stateTypeName } ); ++i )
            {{
               array.{ generationState.DigestWriteMethodName }( ref offset, this._state[i] );
            }}
         }}
         finally
         {{
            if ( instance != null )
            {{
               instance.Array.Array.Clear();
            }}
            AlgorithmUtility.ArrayPool.ReturnInstance( instance );
         }}
      }}

      private void ResetState( Boolean isHashDone )
      {{
         if ( isHashDone )
         {{
            this._expandedState.Clear();
         }}
         var state = this._state;

{ generationState.InitialState.JoinToString( "\n", ( state, idx ) => $"         state[{idx}] = 0x{ state.ToString( "X" ) };" ) }
      }}

      private { unsafeString } void DoTransform(
         Byte* block,
         { stateTypeName }* state,
         { stateTypeName }* x
      )
      {{
         unchecked
         {{
            var i = 0;
            // Populate expanded state beginning
            for (var idx = 0; i < BLOCK_SIZE / sizeof( { stateTypeName } ); ++i )
            {{
               x[i] = { generationState.ExpandedStateReadMethodName }(block, ref idx);
            }}" + ( blockSize / generationState.StateTypeSize < expandedSize ? $@"
            // Expand the rest
            for ( ; i < EXPANDED_STATE_SIZE; ++i)
            {{
               x[i] = { generationState.ExpandCode };
            }}
" : "" ) + $@"

         }}
{ generationState.TransformCodeUnsafe }
      }}

{ generationState.StaticCode }

" + ( useUnsafe ? generationState.StaticCodeOnlyUnsafe : "" ) + $@"
   }}
}}";
      }
   }

   // TODO public data class SerializationInfo<T>( Endianness endianness )

   public static class SHAAlgorithms
   {
      public static SHAAlgorithmGenerator MD5 { get; } = new SHAAlgorithmGenerator( new SHAAlgorithmGenerationStateBuilder()
      {
         AlgorithmName = "MD5",
         DigestByteCount = 0x10,
         BlockSize = 0x40,
         ExpandedSize = 0x10,
         StateTypeName = "UInt32",
         StateTypeSize = 4,
         CountIncreaseForHashEnd = 9,

         InitialState = new UInt64[]
         {
            0x67452301,
            0xefcdab89,
            0x98badcfe,
            0x10325476
         }.ToImmutableArray(),

         LengthWriteMethodName = "WriteInt64LEToBytesNoRef",
         DigestWriteMethodName = "WriteUInt32LEToBytes",
         ExpandedStateReadMethodName = "ReadUInt32LEFromBytes",

         ExpandCode = null,
         TransformCodeUnsafe = TransformCodes.MD5,
         StaticCode = StaticCodes.MD5,
         StaticCodeOnlyUnsafe = SataticCodesUnsafe.MD5,
      } );

      public static SHAAlgorithmGenerator SHA128 { get; } = new SHAAlgorithmGenerator( new SHAAlgorithmGenerationStateBuilder()
      {
         AlgorithmName = "SHA128",
         DigestByteCount = 0x14,
         BlockSize = 0x40,
         ExpandedSize = 0x50,
         StateTypeName = "UInt32",
         StateTypeSize = 4,
         CountIncreaseForHashEnd = 9,

         InitialState = new UInt64[]
         {
            0x67452301,
            0xefcdab89,
            0x98badcfe,
            0x10325476,
            0xc3d2e1f0
         }.ToImmutableArray(),

         LengthWriteMethodName = "WriteInt64BEToBytesNoRef",
         DigestWriteMethodName = "WriteUInt32BEToBytes",
         ExpandedStateReadMethodName = "ReadUInt32BEFromBytes",

         ExpandCode = ExpandCodes.SHA128,
         TransformCodeUnsafe = TransformCodes.SHA128,
         StaticCode = StaticCodes.SHA128,
         StaticCodeOnlyUnsafe = SataticCodesUnsafe.SHA128Or256,
      } );

      public static SHAAlgorithmGenerator SHA256 { get; } = new SHAAlgorithmGenerator( new SHAAlgorithmGenerationStateBuilder()
      {
         AlgorithmName = "SHA256",
         DigestByteCount = 0x20,
         BlockSize = 0x40,
         ExpandedSize = 0x40,
         StateTypeName = "UInt32",
         StateTypeSize = 4,
         CountIncreaseForHashEnd = 9,

         InitialState = new UInt64[]
         {
            // SHA-256 initial hash value:
            // The first 32 bits of the fractional parts of the square roots of the first eight prime numbers
            0x6a09e667,
            0xbb67ae85,
            0x3c6ef372,
            0xa54ff53a,
            0x510e527f,
            0x9b05688c,
            0x1f83d9ab,
            0x5be0cd19
         }.ToImmutableArray(),

         LengthWriteMethodName = "WriteInt64BEToBytesNoRef",
         DigestWriteMethodName = "WriteUInt32BEToBytes",
         ExpandedStateReadMethodName = "ReadUInt32BEFromBytes",

         ExpandCode = ExpandCodes.SHA256,
         TransformCodeUnsafe = TransformCodes.SHA256Or384Or512,
         StaticCode = StaticCodes.SHA256,
         StaticCodeOnlyUnsafe = SataticCodesUnsafe.SHA128Or256,
      } );

      public static SHAAlgorithmGenerator SHA384 { get; } = new SHAAlgorithmGenerator( new SHAAlgorithmGenerationStateBuilder()
      {
         AlgorithmName = "SHA384",
         DigestByteCount = 0x30,
         BlockSize = 0x80,
         ExpandedSize = 0x50,
         StateTypeName = "UInt64",
         StateTypeSize = 8,
         CountIncreaseForHashEnd = 17,

         InitialState = new UInt64[]
         {
            // SHA-384 initial hash value:
            // The first 64 bits of the fractional parts of the square roots of the 9th through 16th prime numbers
            0xcbbb9d5dc1059ed8,
            0x629a292a367cd507,
            0x9159015a3070dd17,
            0x152fecd8f70e5939,
            0x67332667ffc00b31,
            0x8eb44a8768581511,
            0xdb0c2e0d64f98fa7,
            0x47b5481dbefa4fa4
         }.ToImmutableArray(),

         LengthWriteMethodName = "WriteInt64BEToBytesNoRef",
         DigestWriteMethodName = "WriteUInt64BEToBytes",
         ExpandedStateReadMethodName = "ReadUInt64BEFromBytes",

         ExpandCode = ExpandCodes.SHA384Or512,
         TransformCodeUnsafe = TransformCodes.SHA256Or384Or512,
         StaticCode = StaticCodes.SHA384Or512,
         StaticCodeOnlyUnsafe = SataticCodesUnsafe.SHA384Or512,
      } );

      public static SHAAlgorithmGenerator SHA512 { get; } = new SHAAlgorithmGenerator( new SHAAlgorithmGenerationStateBuilder()
      {
         AlgorithmName = "SHA512",
         DigestByteCount = 0x40,
         BlockSize = 0x80,
         ExpandedSize = 0x50,
         StateTypeName = "UInt64",
         StateTypeSize = 8,
         CountIncreaseForHashEnd = 17,

         InitialState = new UInt64[]
         {
            // SHA-512 initial hash value:
            // The first 64 bits of the fractional parts of the square roots of the first 8 prime numbers
            0x6a09e667f3bcc908,
            0xbb67ae8584caa73b,
            0x3c6ef372fe94f82b,
            0xa54ff53a5f1d36f1,
            0x510e527fade682d1,
            0x9b05688c2b3e6c1f,
            0x1f83d9abfb41bd6b,
            0x5be0cd19137e2179
         }.ToImmutableArray(),

         LengthWriteMethodName = "WriteInt64BEToBytesNoRef",
         DigestWriteMethodName = "WriteUInt64BEToBytes",
         ExpandedStateReadMethodName = "ReadUInt64BEFromBytes",

         ExpandCode = ExpandCodes.SHA384Or512,
         TransformCodeUnsafe = TransformCodes.SHA256Or384Or512,
         StaticCode = StaticCodes.SHA384Or512,
         StaticCodeOnlyUnsafe = SataticCodesUnsafe.SHA384Or512,
      } );


      private static class StaticCodes
      {
         internal const String MD5 = @"
#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static UInt32 F( UInt32 u, UInt32 v, UInt32 w )
      {
         return ( u & v ) | ( ~u & w );
      }
#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static UInt32 G( UInt32 u, UInt32 v, UInt32 w )
      {
         return ( u & w ) | ( v & ~w );
      }
#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static UInt32 H( UInt32 u, UInt32 v, UInt32 w )
      {
         return u ^ v ^ w;
      }
#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static UInt32 K( UInt32 u, UInt32 v, UInt32 w )
      {
         return v ^ ( u | ~w );
      }
";
         internal const String SHA128 = @"
#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static UInt32 F( UInt32 u, UInt32 v, UInt32 w )
      {
         return ( u & v ) | ( ~u & w );
      }
#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static UInt32 H( UInt32 u, UInt32 v, UInt32 w )
      {
         return u ^ v ^ w;
      }
#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static UInt32 G( UInt32 u, UInt32 v, UInt32 w )
      {
         return ( u & v ) | ( u & w ) | ( v & w );
      }
";
         internal const String SHA256 = @"
      // the first 32 bits of the fractional parts of the cube roots of the first 64 prime numbers
      private static readonly UInt32[] K = new UInt32[EXPANDED_STATE_SIZE]
      {
         0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5,
         0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
         0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3,
         0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
         0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc,
         0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
         0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7,
         0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
         0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13,
         0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
         0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3,
         0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
         0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5,
         0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
         0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208,
         0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
      };
#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static UInt32 Theta0( UInt32 val )
      {
         return val.RotateRight( 7 ) ^ val.RotateRight( 18 ) ^ ( val >> 3 );
      }
#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static UInt32 Theta1( UInt32 val )
      {
         return val.RotateRight( 17 ) ^ val.RotateRight( 19 ) ^ ( val >> 10 );
      }
#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static UInt32 Sum0Maj( UInt32 x, UInt32 y, UInt32 z )
      {
         unchecked
         {
            return ( x.RotateRight( 2 ) ^ x.RotateRight( 13 ) ^ x.RotateRight( 22 ) ) // Sum0
               + ( ( x & y ) ^ ( x & z ) ^ ( y & z ) ); // Maj
         }
      }
#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static UInt32 Sum1Ch( UInt32 x, UInt32 y, UInt32 z )
      {
         unchecked
         {
            return ( x.RotateRight( 6 ) ^ x.RotateRight( 11 ) ^ x.RotateRight( 25 ) ) // Sum1
               + ( ( x & y ) ^ ( ( ~x ) & z ) ); // Ch
         }
      }
";

         internal const String SHA384Or512 = @"
      // The first 64 bits of the fractional parts of the cube roots of the first 64 prime numbers
      private static readonly UInt64[] K = new UInt64[EXPANDED_STATE_SIZE]
      {
         0x428a2f98d728ae22, 0x7137449123ef65cd, 0xb5c0fbcfec4d3b2f, 0xe9b5dba58189dbbc,
         0x3956c25bf348b538, 0x59f111f1b605d019, 0x923f82a4af194f9b, 0xab1c5ed5da6d8118,
         0xd807aa98a3030242, 0x12835b0145706fbe, 0x243185be4ee4b28c, 0x550c7dc3d5ffb4e2,
         0x72be5d74f27b896f, 0x80deb1fe3b1696b1, 0x9bdc06a725c71235, 0xc19bf174cf692694,
         0xe49b69c19ef14ad2, 0xefbe4786384f25e3, 0x0fc19dc68b8cd5b5, 0x240ca1cc77ac9c65,
         0x2de92c6f592b0275, 0x4a7484aa6ea6e483, 0x5cb0a9dcbd41fbd4, 0x76f988da831153b5,
         0x983e5152ee66dfab, 0xa831c66d2db43210, 0xb00327c898fb213f, 0xbf597fc7beef0ee4,
         0xc6e00bf33da88fc2, 0xd5a79147930aa725, 0x06ca6351e003826f, 0x142929670a0e6e70,
         0x27b70a8546d22ffc, 0x2e1b21385c26c926, 0x4d2c6dfc5ac42aed, 0x53380d139d95b3df,
         0x650a73548baf63de, 0x766a0abb3c77b2a8, 0x81c2c92e47edaee6, 0x92722c851482353b,
         0xa2bfe8a14cf10364, 0xa81a664bbc423001, 0xc24b8b70d0f89791, 0xc76c51a30654be30,
         0xd192e819d6ef5218, 0xd69906245565a910, 0xf40e35855771202a, 0x106aa07032bbd1b8,
         0x19a4c116b8d2d0c8, 0x1e376c085141ab53, 0x2748774cdf8eeb99, 0x34b0bcb5e19b48a8,
         0x391c0cb3c5c95a63, 0x4ed8aa4ae3418acb, 0x5b9cca4f7763e373, 0x682e6ff3d6b2b8a3,
         0x748f82ee5defb2fc, 0x78a5636f43172f60, 0x84c87814a1f0ab72, 0x8cc702081a6439ec,
         0x90befffa23631e28, 0xa4506cebde82bde9, 0xbef9a3f7b2c67915, 0xc67178f2e372532b,
         0xca273eceea26619c, 0xd186b8c721c0c207, 0xeada7dd6cde0eb1e, 0xf57d4f7fee6ed178,
         0x06f067aa72176fba, 0x0a637dc5a2c898a6, 0x113f9804bef90dae, 0x1b710b35131c471b,
         0x28db77f523047d84, 0x32caab7b40c72493, 0x3c9ebe0a15c9bebc, 0x431d67c49c100d4c,
         0x4cc5d4becb3e42b6, 0x597f299cfc657e2a, 0x5fcb6fab3ad6faec, 0x6c44198c4a475817
      };

#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static UInt64 Sigma0( UInt64 x )
      {
         return x.RotateRight( 1 ) ^ x.RotateRight( 8 ) ^ ( x >> 7 );
      }

#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static UInt64 Sigma1( UInt64 x )
      {
         return x.RotateRight( 19 ) ^ x.RotateRight( 61 ) ^ ( x >> 6 );
      }

#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static UInt64 Sum0Maj( UInt64 x, UInt64 y, UInt64 z )
      {
         unchecked
         {
            return ( x.RotateRight( 28 ) ^ x.RotateRight( 34 ) ^ x.RotateRight( 39 ) ) // Sum0
               + ( ( x & y ) ^ ( x & z ) ^ ( y & z ) ); // Maj
         }
      }

#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static UInt64 Sum1Ch( UInt64 x, UInt64 y, UInt64 z )
      {
         unchecked
         {
            return ( x.RotateRight( 14 ) ^ x.RotateRight( 18 ) ^ x.RotateRight( 41 ) ) // Sum1
               + ( ( x & y ) ^ ( ( ~x ) & z ) ); // Ch
         }
      }
";
      }

      private static class ExpandCodes
      {
         internal const String SHA128 = "(x[i - 3] ^ x[i - 8] ^ x[i - 14] ^ x[i - 16]).RotateLeft( 1 )";
         internal const String SHA256 = "unchecked( Theta1( x[i - 2] ) + x[i - 7] + Theta0( x[i - 15] ) + x[i - 16] )";
         internal const String SHA384Or512 = "unchecked( Sigma1( x[i - 2] ) + x[i - 7] + Sigma0( x[i - 15] ) + x[i - 16] )";
      }

      private static class SataticCodesUnsafe
      {
         internal const String MD5 = @"
#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static unsafe UInt32 ReadUInt32LEFromBytes( Byte* array, ref Int32 idx )
      {
         idx += 4;
         return unchecked((UInt32)(( array[idx - 1] << 24 ) | ( array[idx - 2] << 16 ) | ( array[idx - 3] << 8 ) | array[idx - 4]));
      }
";

         internal const String SHA128Or256 = @"
#if !NET40
      [System.Runtime.CompilerServices.MethodImpl( System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining )]
#endif
      private static unsafe UInt32 ReadUInt32BEFromBytes( Byte* array, ref Int32 idx )
      {
         idx += 4;
         return unchecked((UInt32) ( ( array[idx - 4] << 24 ) | ( array[idx - 3] << 16 ) | ( array[idx - 2] << 8 ) | array[idx - 1] ));
      }
";

         internal const String SHA384Or512 = @"
#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static unsafe UInt64 ReadUInt64BEFromBytes( Byte* array, ref Int32 idx )
      {
         idx += 8;
         return unchecked((UInt64) ( ( (Int64) ReadInt32BEFromBytesNoRef( array, idx - 8 ) ) << 32 ) | ( ( (UInt32) ReadInt32BEFromBytesNoRef( array, idx - 4 ) ) ) );
      }

#if !NET40
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
      private static unsafe Int32 ReadInt32BEFromBytesNoRef( Byte* array, Int32 idx )
      {
         return ( array[idx] << 24 ) | ( array[idx + 1] << 16 ) | ( array[idx + 2] << 8 ) | array[idx + 3];
      }
";
      }

      internal static class TransformCodes
      {
         internal const String MD5 = @"
         // Shift amounts for cycles
         const Int32 S11 = 7;
         const Int32 S12 = 12;
         const Int32 S13 = 17;
         const Int32 S14 = 22;

         const Int32 S21 = 5;
         const Int32 S22 = 9;
         const Int32 S23 = 14;
         const Int32 S24 = 20;

         const Int32 S31 = 4;
         const Int32 S32 = 11;
         const Int32 S33 = 16;
         const Int32 S34 = 23;

         const Int32 S41 = 6;
         const Int32 S42 = 10;
         const Int32 S43 = 15;
         const Int32 S44 = 21;

         unchecked
         {
            // Prepare variables
            var a = state[0];
            var b = state[1];
            var c = state[2];
            var d = state[3];

            // F cycle
            a = ( a + F( b, c, d ) + x[0] + 0xd76aa478 ).RotateLeft( S11 ) + b;
            d = ( d + F( a, b, c ) + x[1] + 0xe8c7b756 ).RotateLeft( S12 ) + a;
            c = ( c + F( d, a, b ) + x[2] + 0x242070db ).RotateLeft( S13 ) + d;
            b = ( b + F( c, d, a ) + x[3] + 0xc1bdceee ).RotateLeft( S14 ) + c;
            a = ( a + F( b, c, d ) + x[4] + 0xf57c0faf ).RotateLeft( S11 ) + b;
            d = ( d + F( a, b, c ) + x[5] + 0x4787c62a ).RotateLeft( S12 ) + a;
            c = ( c + F( d, a, b ) + x[6] + 0xa8304613 ).RotateLeft( S13 ) + d;
            b = ( b + F( c, d, a ) + x[7] + 0xfd469501 ).RotateLeft( S14 ) + c;
            a = ( a + F( b, c, d ) + x[8] + 0x698098d8 ).RotateLeft( S11 ) + b;
            d = ( d + F( a, b, c ) + x[9] + 0x8b44f7af ).RotateLeft( S12 ) + a;
            c = ( c + F( d, a, b ) + x[10] + 0xffff5bb1 ).RotateLeft( S13 ) + d;
            b = ( b + F( c, d, a ) + x[11] + 0x895cd7be ).RotateLeft( S14 ) + c;
            a = ( a + F( b, c, d ) + x[12] + 0x6b901122 ).RotateLeft( S11 ) + b;
            d = ( d + F( a, b, c ) + x[13] + 0xfd987193 ).RotateLeft( S12 ) + a;
            c = ( c + F( d, a, b ) + x[14] + 0xa679438e ).RotateLeft( S13 ) + d;
            b = ( b + F( c, d, a ) + x[15] + 0x49b40821 ).RotateLeft( S14 ) + c;

            // G cycle
            a = ( a + G( b, c, d ) + x[1] + 0xf61e2562 ).RotateLeft( S21 ) + b;
            d = ( d + G( a, b, c ) + x[6] + 0xc040b340 ).RotateLeft( S22 ) + a;
            c = ( c + G( d, a, b ) + x[11] + 0x265e5a51 ).RotateLeft( S23 ) + d;
            b = ( b + G( c, d, a ) + x[0] + 0xe9b6c7aa ).RotateLeft( S24 ) + c;
            a = ( a + G( b, c, d ) + x[5] + 0xd62f105d ).RotateLeft( S21 ) + b;
            d = ( d + G( a, b, c ) + x[10] + 0x02441453 ).RotateLeft( S22 ) + a;
            c = ( c + G( d, a, b ) + x[15] + 0xd8a1e681 ).RotateLeft( S23 ) + d;
            b = ( b + G( c, d, a ) + x[4] + 0xe7d3fbc8 ).RotateLeft( S24 ) + c;
            a = ( a + G( b, c, d ) + x[9] + 0x21e1cde6 ).RotateLeft( S21 ) + b;
            d = ( d + G( a, b, c ) + x[14] + 0xc33707d6 ).RotateLeft( S22 ) + a;
            c = ( c + G( d, a, b ) + x[3] + 0xf4d50d87 ).RotateLeft( S23 ) + d;
            b = ( b + G( c, d, a ) + x[8] + 0x455a14ed ).RotateLeft( S24 ) + c;
            a = ( a + G( b, c, d ) + x[13] + 0xa9e3e905 ).RotateLeft( S21 ) + b;
            d = ( d + G( a, b, c ) + x[2] + 0xfcefa3f8 ).RotateLeft( S22 ) + a;
            c = ( c + G( d, a, b ) + x[7] + 0x676f02d9 ).RotateLeft( S23 ) + d;
            b = ( b + G( c, d, a ) + x[12] + 0x8d2a4c8a ).RotateLeft( S24 ) + c;

            // H cycle
            a = ( a + H( b, c, d ) + x[5] + 0xfffa3942 ).RotateLeft( S31 ) + b;
            d = ( d + H( a, b, c ) + x[8] + 0x8771f681 ).RotateLeft( S32 ) + a;
            c = ( c + H( d, a, b ) + x[11] + 0x6d9d6122 ).RotateLeft( S33 ) + d;
            b = ( b + H( c, d, a ) + x[14] + 0xfde5380c ).RotateLeft( S34 ) + c;
            a = ( a + H( b, c, d ) + x[1] + 0xa4beea44 ).RotateLeft( S31 ) + b;
            d = ( d + H( a, b, c ) + x[4] + 0x4bdecfa9 ).RotateLeft( S32 ) + a;
            c = ( c + H( d, a, b ) + x[7] + 0xf6bb4b60 ).RotateLeft( S33 ) + d;
            b = ( b + H( c, d, a ) + x[10] + 0xbebfbc70 ).RotateLeft( S34 ) + c;
            a = ( a + H( b, c, d ) + x[13] + 0x289b7ec6 ).RotateLeft( S31 ) + b;
            d = ( d + H( a, b, c ) + x[0] + 0xeaa127fa ).RotateLeft( S32 ) + a;
            c = ( c + H( d, a, b ) + x[3] + 0xd4ef3085 ).RotateLeft( S33 ) + d;
            b = ( b + H( c, d, a ) + x[6] + 0x04881d05 ).RotateLeft( S34 ) + c;
            a = ( a + H( b, c, d ) + x[9] + 0xd9d4d039 ).RotateLeft( S31 ) + b;
            d = ( d + H( a, b, c ) + x[12] + 0xe6db99e5 ).RotateLeft( S32 ) + a;
            c = ( c + H( d, a, b ) + x[15] + 0x1fa27cf8 ).RotateLeft( S33 ) + d;
            b = ( b + H( c, d, a ) + x[2] + 0xc4ac5665 ).RotateLeft( S34 ) + c;

            // K cycle
            a = ( a + K( b, c, d ) + x[0] + 0xf4292244 ).RotateLeft( S41 ) + b;
            d = ( d + K( a, b, c ) + x[7] + 0x432aff97 ).RotateLeft( S42 ) + a;
            c = ( c + K( d, a, b ) + x[14] + 0xab9423a7 ).RotateLeft( S43 ) + d;
            b = ( b + K( c, d, a ) + x[5] + 0xfc93a039 ).RotateLeft( S44 ) + c;
            a = ( a + K( b, c, d ) + x[12] + 0x655b59c3 ).RotateLeft( S41 ) + b;
            d = ( d + K( a, b, c ) + x[3] + 0x8f0ccc92 ).RotateLeft( S42 ) + a;
            c = ( c + K( d, a, b ) + x[10] + 0xffeff47d ).RotateLeft( S43 ) + d;
            b = ( b + K( c, d, a ) + x[1] + 0x85845dd1 ).RotateLeft( S44 ) + c;
            a = ( a + K( b, c, d ) + x[8] + 0x6fa87e4f ).RotateLeft( S41 ) + b;
            d = ( d + K( a, b, c ) + x[15] + 0xfe2ce6e0 ).RotateLeft( S42 ) + a;
            c = ( c + K( d, a, b ) + x[6] + 0xa3014314 ).RotateLeft( S43 ) + d;
            b = ( b + K( c, d, a ) + x[13] + 0x4e0811a1 ).RotateLeft( S44 ) + c;
            a = ( a + K( b, c, d ) + x[4] + 0xf7537e82 ).RotateLeft( S41 ) + b;
            d = ( d + K( a, b, c ) + x[11] + 0xbd3af235 ).RotateLeft( S42 ) + a;
            c = ( c + K( d, a, b ) + x[2] + 0x2ad7d2bb ).RotateLeft( S43 ) + d;
            b = ( b + K( c, d, a ) + x[9] + 0xeb86d391 ).RotateLeft( S44 ) + c;

            state[0] += a;
            state[1] += b;
            state[2] += c;
            state[3] += d;
         }
";

         internal const String SHA128 = @"
         // Consts
         const UInt32 Y1 = 0x5a827999;
         const UInt32 Y2 = 0x6ed9eba1;
         const UInt32 Y3 = 0x8f1bbcdc;
         const UInt32 Y4 = 0xca62c1d6;
         unchecked
         {
            // Prepare variables
            var h1 = state[0];
            var h2 = state[1];
            var h3 = state[2];
            var h4 = state[3];
            var h5 = state[4];

            // Phase 1
            var i = 0;
            for ( ; i < 20; i += 5 )
            {
               h5 += h1.RotateLeft( 5 ) + F( h2, h3, h4 ) + x[i] + Y1;
               h2 = h2.RotateLeft( 30 );

               h4 += h5.RotateLeft( 5 ) + F( h1, h2, h3 ) + x[i + 1] + Y1;
               h1 = h1.RotateLeft( 30 );

               h3 += h4.RotateLeft( 5 ) + F( h5, h1, h2 ) + x[i + 2] + Y1;
               h5 = h5.RotateLeft( 30 );

               h2 += h3.RotateLeft( 5 ) + F( h4, h5, h1 ) + x[i + 3] + Y1;
               h4 = h4.RotateLeft( 30 );

               h1 += h2.RotateLeft( 5 ) + F( h3, h4, h5 ) + x[i + 4] + Y1;
               h3 = h3.RotateLeft( 30 );
            }

            // Phase 2
            for ( ; i < 40; i += 5 )
            {
               h5 += h1.RotateLeft( 5 ) + H( h2, h3, h4 ) + x[i] + Y2;
               h2 = h2.RotateLeft( 30 );

               h4 += h5.RotateLeft( 5 ) + H( h1, h2, h3 ) + x[i + 1] + Y2;
               h1 = h1.RotateLeft( 30 );

               h3 += h4.RotateLeft( 5 ) + H( h5, h1, h2 ) + x[i + 2] + Y2;
               h5 = h5.RotateLeft( 30 );

               h2 += h3.RotateLeft( 5 ) + H( h4, h5, h1 ) + x[i + 3] + Y2;
               h4 = h4.RotateLeft( 30 );

               h1 += h2.RotateLeft( 5 ) + H( h3, h4, h5 ) + x[i + 4] + Y2;
               h3 = h3.RotateLeft( 30 );
            }

            // Phase 3
            for ( ; i < 60; i += 5 )
            {
               h5 += h1.RotateLeft( 5 ) + G( h2, h3, h4 ) + x[i] + Y3;
               h2 = h2.RotateLeft( 30 );

               h4 += h5.RotateLeft( 5 ) + G( h1, h2, h3 ) + x[i + 1] + Y3;
               h1 = h1.RotateLeft( 30 );

               h3 += h4.RotateLeft( 5 ) + G( h5, h1, h2 ) + x[i + 2] + Y3;
               h5 = h5.RotateLeft( 30 );

               h2 += h3.RotateLeft( 5 ) + G( h4, h5, h1 ) + x[i + 3] + Y3;
               h4 = h4.RotateLeft( 30 );

               h1 += h2.RotateLeft( 5 ) + G( h3, h4, h5 ) + x[i + 4] + Y3;
               h3 = h3.RotateLeft( 30 );
            }

            // Phase 4
            for ( ; i < 80; i += 5 )
            {
               h5 += h1.RotateLeft( 5 ) + H( h2, h3, h4 ) + x[i] + Y4;
               h2 = h2.RotateLeft( 30 );

               h4 += h5.RotateLeft( 5 ) + H( h1, h2, h3 ) + x[i + 1] + Y4;
               h1 = h1.RotateLeft( 30 );

               h3 += h4.RotateLeft( 5 ) + H( h5, h1, h2 ) + x[i + 2] + Y4;
               h5 = h5.RotateLeft( 30 );

               h2 += h3.RotateLeft( 5 ) + H( h4, h5, h1 ) + x[i + 3] + Y4;
               h4 = h4.RotateLeft( 30 );

               h1 += h2.RotateLeft( 5 ) + H( h3, h4, h5 ) + x[i + 4] + Y4;
               h3 = h3.RotateLeft( 30 );
            }

            // Update state
            state[0] += h1;
            state[1] += h2;
            state[2] += h3;
            state[3] += h4;
            state[4] += h5;
         }
";

         internal const String SHA256Or384Or512 = @"
         unchecked
         {
            // Prepare variables
            var h1 = state[0];
            var h2 = state[1];
            var h3 = state[2];
            var h4 = state[3];
            var h5 = state[4];
            var h6 = state[5];
            var h7 = state[6];
            var h8 = state[7];

            for ( var i = 0; i < EXPANDED_STATE_SIZE; )
            {
               h8 += Sum1Ch( h5, h6, h7 ) + K[i] + x[i];
               h4 += h8;
               h8 += Sum0Maj( h1, h2, h3 );
               ++i;

               h7 += Sum1Ch( h4, h5, h6 ) + K[i] + x[i];
               h3 += h7;
               h7 += Sum0Maj( h8, h1, h2 );
               ++i;

               h6 += Sum1Ch( h3, h4, h5 ) + K[i] + x[i];
               h2 += h6;
               h6 += Sum0Maj( h7, h8, h1 );
               ++i;

               h5 += Sum1Ch( h2, h3, h4 ) + K[i] + x[i];
               h1 += h5;
               h5 += Sum0Maj( h6, h7, h8 );
               ++i;

               h4 += Sum1Ch( h1, h2, h3 ) + K[i] + x[i];
               h8 += h4;
               h4 += Sum0Maj( h5, h6, h7 );
               ++i;

               h3 += Sum1Ch( h8, h1, h2 ) + K[i] + x[i];
               h7 += h3;
               h3 += Sum0Maj( h4, h5, h6 );
               ++i;

               h2 += Sum1Ch( h7, h8, h1 ) + K[i] + x[i];
               h6 += h2;
               h2 += Sum0Maj( h3, h4, h5 );
               ++i;

               h1 += Sum1Ch( h6, h7, h8 ) + K[i] + x[i];
               h5 += h1;
               h1 += Sum0Maj( h2, h3, h4 );
               ++i;

            }

            // Update state
            state[0] += h1;
            state[1] += h2;
            state[2] += h3;
            state[3] += h4;
            state[4] += h5;
            state[5] += h6;
            state[6] += h7;
            state[7] += h8;
         }
";
      }
   }


}
