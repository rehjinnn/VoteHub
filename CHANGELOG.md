# Changelog — VoteHub UI updates

## 2026-07-15
- Redesigned navbar behavior:
  - Sticky navbar with dynamic height (`--nav-height`), smooth transparent→solid transition on scroll, backdrop blur, soft shadow, and thin bottom border.
  - Increased navbar z-index to 2000 and ensured toggler is on top for mobile.
- Added MSU color palette and semantic utility classes in `wwwroot/css/site.css` (`.btn-msu`, `.bg-msu`, `.text-msu`, `.bg-sage`, etc.).
- Replaced Bootstrap blue utilities across Razor views with MSU equivalents (16 view files updated).
- Strengthened hero overlay for improved text contrast and adjusted hero styles.
- Added JS to measure navbar height and toggle `.navbar-scrolled`, with resize debounce.
- Captured desktop and mobile screenshots and performed basic responsive checks locally.

If you want, I can create a Git commit and prepare a PR with these changes next.
