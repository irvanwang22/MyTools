# MyTools

General-purpose Unity utilities (namespace `MyTools`).

## Install

Via git URL in `Packages/manifest.json`:

```json
"com.irvanwang.mytools": "https://github.com/irvanwang22/MyTools.git#v1.0.0"
```

Or as an editable embedded package (git submodule):

```
git submodule add https://github.com/irvanwang22/MyTools.git Packages/com.irvanwang.mytools
```

## Assemblies

| Assembly | Contents | Notes |
|---|---|---|
| `MyTools` | Runtime utilities | |
| `MyTools.DOTween` | `PunchEffect` | Compiled only when the `DOTWEEN` define is set (added automatically by DOTween setup) |
| `MyTools.Editor` | Editor tools, inspectors, property drawers | Editor only |

## Dependencies

- uGUI / TextMeshPro (`com.unity.ugui`)
- Mathematics (`com.unity.mathematics`)
- Localization (`com.unity.localization`)
- AI module (`com.unity.modules.ai`)
- Optional: DOTween (not a UPM package, install separately)
