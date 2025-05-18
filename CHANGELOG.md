# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.4] - 2025-05-18

### Added
- Support for overriding monster stats using `.toml` config files, in addition to JSON.

### Fixed
- Guardians of the Tree were not reviving properly after defeat. This bug has been fixed.

## [1.0.3] - 2025-05-14

### Added

- Revive button added for **Choirmaster, the Inspector**.

### Changed

- The black screen during boss refresh is now optional, in an attempt to work around the rare BlackScreen.Instance missing method error reported by some users.

## [1.0.2] - 2025-05-11

### Fixed

- **Ursula, the Mutated** could freeze after defeat and fail to drop rewards. This has been fixed.
- **Persephone, the Stygian Queen of Woe** was invisible after reviving during the encounter. She now properly reappears.


## [1.0.1] - 2025-05-09

### Fixed

- Ursula’s second form now always drops the Mutant Blader Nucleus.
- Resolved game freeze issue when reviving and rebattling cutscene-triggered bosses by forcing a scene reload.
