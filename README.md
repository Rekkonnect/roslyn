# Implementation Tracking

The features are (mostly) ordered by discovery date, and partly grouped by relevance. There is no fixed implementation order schedule.

## Clarity

| Feature Description | Issue(s) | Prototype | Spec | Importance | Relevance | Difficulty |
|---------------------|----------|:---------:|:----:|:----------:|:---------:|:----------:|
| Breaking parsing on extra parenthesis in local method | [#58529](https://github.com/dotnet/roslyn/issues/58529) | Uninitialized | N/A | 2.48 | 5.06 | 2.34 |
| Inconsistent type argument error locations | [#65515](https://github.com/dotnet/roslyn/issues/65515) | TBD | N/A | 1.63 | 4.87 | 1.90 |
| `DiagnosticAnalyzerAttribute` on abstract classes | [#56340](https://github.com/dotnet/roslyn/issues/56340) | Uninitialized | N/A | 0.29 | 0.95 | 1.16 |
| Poor error recovery after missing `>` | [#24642](https://github.com/dotnet/roslyn/issues/24642) | Uninitialized | N/A | 1.87 | 2.16 | 3.02 |
| Preprocessor directive reference lookup | [#66009](https://github.com/dotnet/roslyn/issues/66009) | [Pending Review](https://github.com/dotnet/roslyn/pull/66425) | N/A | 2.21 | 0.58 | 1.70 |
| Pointer fields on records errors | [#66312](https://github.com/dotnet/roslyn/issues/66312) | Uninitialized | N/A | 1.06 | 0.57 | 0.93 |

## Metrics Legend
All metric values are based on an exponential (base 1.5) score, and are uncapped. Scores are estimated considering existing compiler features, normalized such that the median metric value is 6.40 for existing major compiler features (records, `init`, etc.).

- ***Importance*** refers to how impactful and desired an implementation is, to its intended target users
- ***Relevance*** refers to the size of the target user group, including the ones that will make use of the implementation
- ***Difficulty*** refers to the expected difficulty of the implementation, including its estimated implementation time
