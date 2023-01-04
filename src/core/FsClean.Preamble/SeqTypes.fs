namespace FsClean

type FoldCondition<'a> =
    | Continue
    | ContinueWith of 'a
    | Break
    | BreakWith of 'a
