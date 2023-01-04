namespace FsClean.Application.Serialization

open System
open System.Reflection

open FsClean

[<AbstractClass; AllowNullLiteral; AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)>]
type BaseTypeSchemaAttribute() =
    inherit Attribute()
    abstract Schema : string

[<AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)>]
type TypeSchemaAttribute(schema) =
    inherit BaseTypeSchemaAttribute()
    override this.Schema = schema

module TypeSchemaConverter =
    let ofTypes (types: Type seq) : DualFn<Type, String> =
        types
        |> Seq.collect (fun t ->
            let attr =
                t.GetCustomAttribute<BaseTypeSchemaAttribute>(false)

            if isNull attr then
                List.empty
            else
                [ t, attr.Schema ])
        |> DualFn.ofPairsDict
