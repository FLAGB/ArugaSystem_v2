# Aruga database scripts

Run these in SQL Server Management Studio (open the file, press **Execute**).

| Script | When to use it |
|---|---|
| `Schema.sql` | **New computer / empty SQL Server.** Creates `ArugaSystemDB` with every table, the 7 EPI vaccines and their schedule, clinic hours (Mon–Fri 8 AM–5 PM), 3 vaccination rooms (Room 1–3), and one admin account: **admin / Admin@2026** (asks for a new password at first login). |
| `Cleanup_2026-09.sql` | **You already have an older `ArugaSystemDB`.** Upgrades it to the same structure as `Schema.sql` without touching your patients or records. Back up first (right-click the database → Tasks → Back Up...). |
| `DemoSeed.sql` | Optional. Adds sample families, children, vaccination history, stock and today's queue for demos. Run it **on the morning of the demo**: dates are computed from the day you run it. Safe to run again. |
| `DemoSeed_Remove.sql` | Removes everything `DemoSeed.sql` added. Your own records are left alone. |

Order on a new machine: `Schema.sql` → (optional) `DemoSeed.sql`.
Order on an existing database: back up → `Cleanup_2026-09.sql` → (optional) `DemoSeed.sql`.

Notes
- SSMS shows "Caution: Changing any part of an object name could break scripts..." while `Cleanup_2026-09.sql` gives constraints readable names. That is expected.
- From the command line, add `-I` so SQL Server accepts the scripts:
  `sqlcmd -S localhost -E -I -d ArugaSystemDB -i DemoSeed.sql`
- Demo logins (password **Aruga@2026**): `demo.admin`, `demo.doctor`, `demo.nurse`, `demo.nurse2`, `demo.staff`, and parents such as `maria.santos@demo.aruga.ph`.
- The seed runs on a weekend too, but the clinic is closed on weekends, so nothing is "due today". Run it on a clinic day, or add the day under System Admin → Operating Hours → Add Exception.
