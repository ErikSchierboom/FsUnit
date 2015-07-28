﻿// TODO : Add license header

namespace FsUnit

open System
open NUnit.Framework
open NUnit.Framework.Constraints

type ChoiceConstraint(n) =
  inherit Constraint() with
    override this.WriteDescriptionTo(writer: MessageWriter): unit =
      writer.WritePredicate("is choice")
      writer.WriteExpectedValue(sprintf "%d" n)
    override this.Matches(actual: obj) =
      match actual with
        | null -> raise (new ArgumentException("The actual value must be a non-null choice"))
        | o -> (new CustomMatchers.ChoiceDiscriminator(n)).check(o)

/// F#-friendly formatting for otherwise the same equals behavior (%A instead of .ToString())
type EqualsConstraint(x:obj) =
  inherit EqualConstraint(x) with
    override this.WriteActualValueTo(writer: MessageWriter): unit =
      writer.WriteActualValue(sprintf "%A" this.actual)
    override this.WriteDescriptionTo(writer: MessageWriter): unit =
      writer.WritePredicate("equals")
      writer.WriteExpectedValue(sprintf "%A" x)
    override this.WriteMessageTo(writer: MessageWriter): unit =
      writer.WriteMessageLine(sprintf "Expected: %A, but was %A" x this.actual)

//
[<AutoOpen>]
module TopLevelOperators =
    let Null = NullConstraint()

    let Empty = EmptyConstraint()

    let EmptyString = EmptyStringConstraint()

    let NullOrEmptyString = NullOrEmptyStringConstraint()

    let True = TrueConstraint()

    let False = FalseConstraint()

    let NaN = NaNConstraint()

    let unique = UniqueItemsConstraint()

    let should (f : 'a -> #Constraint) x (y : obj) =
        let c = f x
        let y =
            match y with
            | :? (unit -> unit) -> box (TestDelegate(y :?> unit -> unit))
            | _ -> y
        Assert.That(y, c)
    
    let equal x = EqualsConstraint(x)

    let equalWithin tolerance x = equal(x).Within tolerance

    let contain x = ContainsConstraint(x)

    let haveLength n = Has.Length.EqualTo(n)

    let haveCount n = Has.Count.EqualTo(n)

    let be = id

    let sameAs x = SameAsConstraint(x)

    let throw = Throws.TypeOf

    let throwWithMessage (m:string) (t:System.Type) = Throws.TypeOf(t).And.Message.EqualTo(m)

    let greaterThan x = GreaterThanConstraint(x)

    let greaterThanOrEqualTo x = GreaterThanOrEqualConstraint(x)

    let lessThan x = LessThanConstraint(x)

    let lessThanOrEqualTo x = LessThanOrEqualConstraint(x)

    let shouldFail (f : unit -> unit) =
        TestDelegate(f) |> should throw typeof<AssertionException>

    let endWith (s:string) = EndsWithConstraint s

    let startWith (s:string) = StartsWithConstraint s

    let ofExactType<'a> = ExactTypeConstraint(typeof<'a>)

    let instanceOfType<'a> = InstanceOfTypeConstraint(typeof<'a>)

    let choice n = ChoiceConstraint(n)

    let ascending = Is.Ordered

    let descending = Is.Ordered.Descending

    let not' x = NotConstraint(x)

    /// Deprecated operators. These will be removed in a future version of FsUnit.
    module FsUnitDepricated =
        [<System.Obsolete>]
        let not x = not' x
