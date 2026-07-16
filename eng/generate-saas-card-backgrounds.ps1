[CmdletBinding(SupportsShouldProcess, ConfirmImpact = 'Medium')]
param(
    [string]$CatalogPath = (Join-Path $PSScriptRoot '..\Modules\Jalium.UI.Gallery.Modules.Main\Views\Pages\GalleryComponentCatalog.cs'),
    [string]$OutputDirectory = (Join-Path $PSScriptRoot '..\Modules\Jalium.UI.Gallery.Modules.Main\Assets\SaasCardBackgrounds'),
    [int]$Width = 384,
    [int]$Height = 208
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Drawing

function Get-StableBytes {
    param([Parameter(Mandatory)][string]$Value)

    $sha = [System.Security.Cryptography.SHA256]::Create()
    try {
        return $sha.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($Value))
    }
    finally {
        $sha.Dispose()
    }
}

function Convert-HexColor {
    param([Parameter(Mandatory)][string]$Value)
    return [System.Drawing.ColorTranslator]::FromHtml($Value)
}

function With-Alpha {
    param(
        [Parameter(Mandatory)][System.Drawing.Color]$Color,
        [Parameter(Mandatory)][int]$Alpha
    )
    return [System.Drawing.Color]::FromArgb($Alpha, $Color.R, $Color.G, $Color.B)
}

function New-RoundedRectanglePath {
    param(
        [Parameter(Mandatory)][System.Drawing.RectangleF]$Rectangle,
        [Parameter(Mandatory)][float]$Radius
    )

    $diameter = [Math]::Max(1.0, $Radius * 2.0)
    $arc = [System.Drawing.RectangleF]::new($Rectangle.X, $Rectangle.Y, $diameter, $diameter)
    $path = [System.Drawing.Drawing2D.GraphicsPath]::new()
    $path.AddArc($arc, 180, 90)
    $arc.X = $Rectangle.Right - $diameter
    $path.AddArc($arc, 270, 90)
    $arc.Y = $Rectangle.Bottom - $diameter
    $path.AddArc($arc, 0, 90)
    $arc.X = $Rectangle.Left
    $path.AddArc($arc, 90, 90)
    $path.CloseFigure()
    return $path
}

function Fill-RoundedRectangle {
    param(
        [Parameter(Mandatory)][System.Drawing.Graphics]$Graphics,
        [Parameter(Mandatory)][System.Drawing.Brush]$Brush,
        [Parameter(Mandatory)][System.Drawing.RectangleF]$Rectangle,
        [Parameter(Mandatory)][float]$Radius
    )

    $path = New-RoundedRectanglePath -Rectangle $Rectangle -Radius $Radius
    try {
        $Graphics.FillPath($Brush, $path)
    }
    finally {
        $path.Dispose()
    }
}

function Draw-RoundedRectangle {
    param(
        [Parameter(Mandatory)][System.Drawing.Graphics]$Graphics,
        [Parameter(Mandatory)][System.Drawing.Pen]$Pen,
        [Parameter(Mandatory)][System.Drawing.RectangleF]$Rectangle,
        [Parameter(Mandatory)][float]$Radius
    )

    $path = New-RoundedRectanglePath -Rectangle $Rectangle -Radius $Radius
    try {
        $Graphics.DrawPath($Pen, $path)
    }
    finally {
        $path.Dispose()
    }
}

function Get-CategoryPalette {
    param([Parameter(Mandatory)][string]$Category)

    $palettes = @{
        Controls   = @('#071D2A', '#0B5360', '#21D4C2', '#8CF5E7')
        Text       = @('#111535', '#3730A3', '#818CF8', '#C7D2FE')
        Layout     = @('#071C31', '#075985', '#38BDF8', '#BAE6FD')
        Navigation = @('#06281F', '#047857', '#34D399', '#A7F3D0')
        Data       = @('#2B1808', '#9A3412', '#FB923C', '#FED7AA')
        Media      = @('#30101E', '#9F1239', '#FB7185', '#FECDD3')
        Visuals    = @('#21103A', '#6D28D9', '#A78BFA', '#DDD6FE')
        System     = @('#111827', '#334155', '#94A3B8', '#E2E8F0')
    }

    return $palettes[$Category]
}

function Draw-Glow {
    param(
        [Parameter(Mandatory)][System.Drawing.Graphics]$Graphics,
        [Parameter(Mandatory)][System.Drawing.Color]$Color,
        [Parameter(Mandatory)][float]$X,
        [Parameter(Mandatory)][float]$Y,
        [Parameter(Mandatory)][float]$Radius
    )

    foreach ($layer in 0..5) {
        $layerRadius = $Radius * (1.0 - ($layer * 0.12))
        $alpha = 6 + ($layer * 4)
        $brush = [System.Drawing.SolidBrush]::new((With-Alpha -Color $Color -Alpha $alpha))
        try {
            $Graphics.FillEllipse($brush, $X - $layerRadius, $Y - $layerRadius, $layerRadius * 2, $layerRadius * 2)
        }
        finally {
            $brush.Dispose()
        }
    }
}

function Draw-ControlsMotif {
    param($Graphics, $Bytes, $Accent, $Pale)

    $panel = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(24, 255, 255, 255))
    $stroke = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(42, 255, 255, 255), 1.0)
    $accentBrush = [System.Drawing.SolidBrush]::new((With-Alpha -Color $Accent -Alpha 210))
    try {
        foreach ($index in 0..2) {
            $rect = [System.Drawing.RectangleF]::new(158, 78 + ($index * 27), 144 - ($Bytes[$index + 5] % 24), 17)
            Fill-RoundedRectangle -Graphics $Graphics -Brush $panel -Rectangle $rect -Radius 5
            Draw-RoundedRectangle -Graphics $Graphics -Pen $stroke -Rectangle $rect -Radius 5
            $lineWidth = 32 + ($Bytes[$index + 9] % 42)
            $line = [System.Drawing.RectangleF]::new(166, 84 + ($index * 27), $lineWidth, 5)
            Fill-RoundedRectangle -Graphics $Graphics -Brush $accentBrush -Rectangle $line -Radius 2.5
        }

        $toggleTrack = [System.Drawing.RectangleF]::new(268, 146, 32, 14)
        Fill-RoundedRectangle -Graphics $Graphics -Brush $accentBrush -Rectangle $toggleTrack -Radius 7
        $knob = [System.Drawing.SolidBrush]::new((With-Alpha -Color $Pale -Alpha 245))
        try { $Graphics.FillEllipse($knob, 287, 148, 10, 10) } finally { $knob.Dispose() }
    }
    finally {
        $panel.Dispose()
        $stroke.Dispose()
        $accentBrush.Dispose()
    }
}

function Draw-TextMotif {
    param($Graphics, $Bytes, $Accent, $Pale)

    $paper = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(28, 255, 255, 255))
    $accentBrush = [System.Drawing.SolidBrush]::new((With-Alpha -Color $Accent -Alpha 220))
    $lineBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(82, 255, 255, 255))
    try {
        $page = [System.Drawing.RectangleF]::new(160, 70, 132, 94)
        Fill-RoundedRectangle -Graphics $Graphics -Brush $paper -Rectangle $page -Radius 7
        $Graphics.FillRectangle($accentBrush, 160, 70, 5, 94)
        foreach ($index in 0..4) {
            $width = 54 + ($Bytes[$index + 7] % 57)
            $rect = [System.Drawing.RectangleF]::new(177, 84 + ($index * 14), $width, 4)
            Fill-RoundedRectangle -Graphics $Graphics -Brush $lineBrush -Rectangle $rect -Radius 2
        }
        $cursor = [System.Drawing.Pen]::new((With-Alpha -Color $Pale -Alpha 225), 2)
        try { $Graphics.DrawLine($cursor, 177, 146, 177, 157) } finally { $cursor.Dispose() }
    }
    finally {
        $paper.Dispose()
        $accentBrush.Dispose()
        $lineBrush.Dispose()
    }
}

function Draw-LayoutMotif {
    param($Graphics, $Bytes, $Accent, $Pale)

    $stroke = [System.Drawing.Pen]::new((With-Alpha -Color $Pale -Alpha 125), 1.2)
    $accentBrush = [System.Drawing.SolidBrush]::new((With-Alpha -Color $Accent -Alpha 105))
    $panel = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(22, 255, 255, 255))
    try {
        $outer = [System.Drawing.RectangleF]::new(158, 72, 142, 92)
        Fill-RoundedRectangle -Graphics $Graphics -Brush $panel -Rectangle $outer -Radius 8
        Draw-RoundedRectangle -Graphics $Graphics -Pen $stroke -Rectangle $outer -Radius 8
        $leftWidth = 44 + ($Bytes[8] % 18)
        $left = [System.Drawing.RectangleF]::new(168, 83, $leftWidth, 70)
        Fill-RoundedRectangle -Graphics $Graphics -Brush $accentBrush -Rectangle $left -Radius 5
        $rightX = 176 + $leftWidth
        foreach ($index in 0..1) {
            $right = [System.Drawing.RectangleF]::new($rightX, 83 + ($index * 38), 112 - $leftWidth, 32)
            Fill-RoundedRectangle -Graphics $Graphics -Brush $panel -Rectangle $right -Radius 5
            Draw-RoundedRectangle -Graphics $Graphics -Pen $stroke -Rectangle $right -Radius 5
        }
    }
    finally {
        $stroke.Dispose()
        $accentBrush.Dispose()
        $panel.Dispose()
    }
}

function Draw-NavigationMotif {
    param($Graphics, $Bytes, $Accent, $Pale)

    $linePen = [System.Drawing.Pen]::new((With-Alpha -Color $Pale -Alpha 92), 1.3)
    $nodeBrush = [System.Drawing.SolidBrush]::new((With-Alpha -Color $Accent -Alpha 220))
    $softBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(34, 255, 255, 255))
    try {
        $points = @(
            [System.Drawing.PointF]::new(171, 91 + ($Bytes[4] % 10)),
            [System.Drawing.PointF]::new(221, 78 + ($Bytes[5] % 15)),
            [System.Drawing.PointF]::new(277, 105 + ($Bytes[6] % 18)),
            [System.Drawing.PointF]::new(218, 148 + ($Bytes[7] % 8))
        )
        $Graphics.DrawLines($linePen, $points)
        foreach ($point in $points) {
            $Graphics.FillEllipse($softBrush, $point.X - 9, $point.Y - 9, 18, 18)
            $Graphics.FillEllipse($nodeBrush, $point.X - 4, $point.Y - 4, 8, 8)
        }
    }
    finally {
        $linePen.Dispose()
        $nodeBrush.Dispose()
        $softBrush.Dispose()
    }
}

function Draw-DataMotif {
    param($Graphics, $Bytes, $Accent, $Pale)

    $panel = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(25, 255, 255, 255))
    $rowPen = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(38, 255, 255, 255), 1)
    $accentBrush = [System.Drawing.SolidBrush]::new((With-Alpha -Color $Accent -Alpha 205))
    $dotBrush = [System.Drawing.SolidBrush]::new((With-Alpha -Color $Pale -Alpha 180))
    try {
        $table = [System.Drawing.RectangleF]::new(155, 72, 151, 94)
        Fill-RoundedRectangle -Graphics $Graphics -Brush $panel -Rectangle $table -Radius 7
        foreach ($index in 1..4) {
            $Graphics.DrawLine($rowPen, 155, 72 + ($index * 19), 306, 72 + ($index * 19))
        }
        foreach ($index in 0..3) {
            $Graphics.FillEllipse($dotBrush, 165, 80 + ($index * 19), 5, 5)
            $bar = [System.Drawing.RectangleF]::new(178, 80 + ($index * 19), 44 + ($Bytes[$index + 11] % 65), 5)
            Fill-RoundedRectangle -Graphics $Graphics -Brush $accentBrush -Rectangle $bar -Radius 2.5
        }
    }
    finally {
        $panel.Dispose()
        $rowPen.Dispose()
        $accentBrush.Dispose()
        $dotBrush.Dispose()
    }
}

function Draw-MediaMotif {
    param($Graphics, $Bytes, $Accent, $Pale)

    $panel = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(29, 255, 255, 255))
    $accentBrush = [System.Drawing.SolidBrush]::new((With-Alpha -Color $Accent -Alpha 150))
    $paleBrush = [System.Drawing.SolidBrush]::new((With-Alpha -Color $Pale -Alpha 215))
    try {
        $frame = [System.Drawing.RectangleF]::new(156, 70, 150, 96)
        Fill-RoundedRectangle -Graphics $Graphics -Brush $panel -Rectangle $frame -Radius 9
        $Graphics.FillEllipse($paleBrush, 174 + ($Bytes[4] % 10), 84, 14, 14)
        $mountains = @(
            [System.Drawing.PointF]::new(164, 151),
            [System.Drawing.PointF]::new(204, 113 + ($Bytes[6] % 12)),
            [System.Drawing.PointF]::new(229, 143),
            [System.Drawing.PointF]::new(264, 103 + ($Bytes[7] % 18)),
            [System.Drawing.PointF]::new(298, 151)
        )
        $Graphics.FillPolygon($accentBrush, $mountains)
        $play = @(
            [System.Drawing.PointF]::new(229, 102),
            [System.Drawing.PointF]::new(229, 128),
            [System.Drawing.PointF]::new(250, 115)
        )
        $Graphics.FillPolygon($paleBrush, $play)
    }
    finally {
        $panel.Dispose()
        $accentBrush.Dispose()
        $paleBrush.Dispose()
    }
}

function Draw-VisualsMotif {
    param($Graphics, $Bytes, $Accent, $Pale)

    $gridPen = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(31, 255, 255, 255), 1)
    $barBrush = [System.Drawing.SolidBrush]::new((With-Alpha -Color $Accent -Alpha 118))
    $linePen = [System.Drawing.Pen]::new((With-Alpha -Color $Pale -Alpha 225), 2.2)
    $dotBrush = [System.Drawing.SolidBrush]::new((With-Alpha -Color $Pale -Alpha 245))
    try {
        foreach ($index in 0..3) {
            $Graphics.DrawLine($gridPen, 157, 88 + ($index * 20), 307, 88 + ($index * 20))
        }
        foreach ($index in 0..4) {
            $height = 20 + ($Bytes[$index + 8] % 48)
            $bar = [System.Drawing.RectangleF]::new(164 + ($index * 28), 159 - $height, 13, $height)
            Fill-RoundedRectangle -Graphics $Graphics -Brush $barBrush -Rectangle $bar -Radius 4
        }
        $points = [System.Drawing.PointF[]]@()
        foreach ($index in 0..5) {
            $points += [System.Drawing.PointF]::new(164 + ($index * 27), 145 - ($Bytes[$index + 14] % 54))
        }
        $Graphics.DrawLines($linePen, $points)
        foreach ($point in $points) { $Graphics.FillEllipse($dotBrush, $point.X - 2.5, $point.Y - 2.5, 5, 5) }
    }
    finally {
        $gridPen.Dispose()
        $barBrush.Dispose()
        $linePen.Dispose()
        $dotBrush.Dispose()
    }
}

function Draw-SystemMotif {
    param($Graphics, $Bytes, $Accent, $Pale)

    $panel = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(24, 255, 255, 255))
    $lineBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(72, 255, 255, 255))
    $accentBrush = [System.Drawing.SolidBrush]::new((With-Alpha -Color $Accent -Alpha 205))
    $knobBrush = [System.Drawing.SolidBrush]::new((With-Alpha -Color $Pale -Alpha 240))
    try {
        $settings = [System.Drawing.RectangleF]::new(155, 72, 151, 94)
        Fill-RoundedRectangle -Graphics $Graphics -Brush $panel -Rectangle $settings -Radius 8
        foreach ($index in 0..2) {
            $y = 88 + ($index * 26)
            $line = [System.Drawing.RectangleF]::new(167, $y, 52 + ($Bytes[$index + 5] % 35), 5)
            Fill-RoundedRectangle -Graphics $Graphics -Brush $lineBrush -Rectangle $line -Radius 2.5
            $track = [System.Drawing.RectangleF]::new(263, $y - 4, 30, 14)
            Fill-RoundedRectangle -Graphics $Graphics -Brush $accentBrush -Rectangle $track -Radius 7
            $knobX = if (($Bytes[$index + 12] % 2) -eq 0) { 265 } else { 281 }
            $Graphics.FillEllipse($knobBrush, $knobX, $y - 2, 10, 10)
        }
    }
    finally {
        $panel.Dispose()
        $lineBrush.Dispose()
        $accentBrush.Dispose()
        $knobBrush.Dispose()
    }
}

function New-SaasBackground {
    param(
        [Parameter(Mandatory)][string]$PageTag,
        [Parameter(Mandatory)][string]$Category,
        [Parameter(Mandatory)][string]$Path
    )

    $bytes = Get-StableBytes -Value $PageTag
    $palette = Get-CategoryPalette -Category $Category
    if ($null -eq $palette) {
        throw "No SaaS palette is defined for category '$Category'."
    }

    $deep = Convert-HexColor $palette[0]
    $mid = Convert-HexColor $palette[1]
    $accent = Convert-HexColor $palette[2]
    $pale = Convert-HexColor $palette[3]

    $bitmap = [System.Drawing.Bitmap]::new($Width, $Height, [System.Drawing.Imaging.PixelFormat]::Format32bppPArgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    try {
        $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
        $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        $graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality

        $bounds = [System.Drawing.Rectangle]::new(0, 0, $Width, $Height)
        $angle = 18 + ($bytes[0] % 48)
        $background = [System.Drawing.Drawing2D.LinearGradientBrush]::new($bounds, $deep, $mid, [float]$angle)
        try { $graphics.FillRectangle($background, $bounds) } finally { $background.Dispose() }

        Draw-Glow -Graphics $graphics -Color $accent -X (54 + ($bytes[1] % 50)) -Y (38 + ($bytes[2] % 40)) -Radius (74 + ($bytes[3] % 34))
        Draw-Glow -Graphics $graphics -Color $pale -X (300 + ($bytes[4] % 48)) -Y (144 + ($bytes[5] % 42)) -Radius (68 + ($bytes[6] % 30))

        $gridPen = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(12, 255, 255, 255), 1)
        try {
            $offsetX = $bytes[7] % 24
            $offsetY = $bytes[8] % 20
            for ($x = $offsetX; $x -lt $Width; $x += 24) { $graphics.DrawLine($gridPen, $x, 0, $x, $Height) }
            for ($y = $offsetY; $y -lt $Height; $y += 20) { $graphics.DrawLine($gridPen, 0, $y, $Width, $y) }
        }
        finally { $gridPen.Dispose() }

        $windowX = 31 + ($bytes[9] % 7)
        $windowY = 21 + ($bytes[10] % 6)
        $window = [System.Drawing.RectangleF]::new($windowX, $windowY, 322, 169)
        $glassBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(189, 7, 14, 27))
        $glassPen = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(58, 255, 255, 255), 1.1)
        try {
            Fill-RoundedRectangle -Graphics $graphics -Brush $glassBrush -Rectangle $window -Radius 13
            Draw-RoundedRectangle -Graphics $graphics -Pen $glassPen -Rectangle $window -Radius 13
        }
        finally {
            $glassBrush.Dispose()
            $glassPen.Dispose()
        }

        $topBarBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(20, 255, 255, 255))
        try {
            $topBar = [System.Drawing.RectangleF]::new($windowX + 1, $windowY + 1, 320, 27)
            Fill-RoundedRectangle -Graphics $graphics -Brush $topBarBrush -Rectangle $topBar -Radius 12
        }
        finally { $topBarBrush.Dispose() }

        $dotColors = @(
            [System.Drawing.Color]::FromArgb(205, 251, 113, 133),
            [System.Drawing.Color]::FromArgb(205, 251, 191, 36),
            [System.Drawing.Color]::FromArgb(205, 52, 211, 153)
        )
        foreach ($index in 0..2) {
            $dot = [System.Drawing.SolidBrush]::new($dotColors[$index])
            try { $graphics.FillEllipse($dot, $windowX + 13 + ($index * 13), $windowY + 11, 6, 6) } finally { $dot.Dispose() }
        }

        $searchBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(27, 255, 255, 255))
        try {
            $search = [System.Drawing.RectangleF]::new($windowX + 225, $windowY + 8, 78, 11)
            Fill-RoundedRectangle -Graphics $graphics -Brush $searchBrush -Rectangle $search -Radius 5.5
        }
        finally { $searchBrush.Dispose() }

        $sidebarBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(23, 255, 255, 255))
        $activeBrush = [System.Drawing.SolidBrush]::new((With-Alpha -Color $accent -Alpha 98))
        $mutedBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(59, 255, 255, 255))
        try {
            $sidebar = [System.Drawing.RectangleF]::new($windowX + 12, $windowY + 39, 88, 116)
            Fill-RoundedRectangle -Graphics $graphics -Brush $sidebarBrush -Rectangle $sidebar -Radius 8
            $activeIndex = $bytes[11] % 4
            foreach ($index in 0..3) {
                $y = $windowY + 52 + ($index * 23)
                if ($index -eq $activeIndex) {
                    $active = [System.Drawing.RectangleF]::new($windowX + 21, $y - 5, 69, 16)
                    Fill-RoundedRectangle -Graphics $graphics -Brush $activeBrush -Rectangle $active -Radius 5
                }
                $graphics.FillEllipse($mutedBrush, $windowX + 28, $y, 5, 5)
                $line = [System.Drawing.RectangleF]::new($windowX + 40, $y, 27 + ($bytes[$index + 12] % 22), 5)
                Fill-RoundedRectangle -Graphics $graphics -Brush $mutedBrush -Rectangle $line -Radius 2.5
            }
        }
        finally {
            $sidebarBrush.Dispose()
            $activeBrush.Dispose()
            $mutedBrush.Dispose()
        }

        switch ($Category) {
            'Controls'   { Draw-ControlsMotif -Graphics $graphics -Bytes $bytes -Accent $accent -Pale $pale }
            'Text'       { Draw-TextMotif -Graphics $graphics -Bytes $bytes -Accent $accent -Pale $pale }
            'Layout'     { Draw-LayoutMotif -Graphics $graphics -Bytes $bytes -Accent $accent -Pale $pale }
            'Navigation' { Draw-NavigationMotif -Graphics $graphics -Bytes $bytes -Accent $accent -Pale $pale }
            'Data'       { Draw-DataMotif -Graphics $graphics -Bytes $bytes -Accent $accent -Pale $pale }
            'Media'      { Draw-MediaMotif -Graphics $graphics -Bytes $bytes -Accent $accent -Pale $pale }
            'Visuals'    { Draw-VisualsMotif -Graphics $graphics -Bytes $bytes -Accent $accent -Pale $pale }
            'System'     { Draw-SystemMotif -Graphics $graphics -Bytes $bytes -Accent $accent -Pale $pale }
        }

        $sparkleBrush = [System.Drawing.SolidBrush]::new((With-Alpha -Color $pale -Alpha 150))
        try {
            foreach ($index in 0..2) {
                $x = 18 + ($bytes[$index + 20] % 350)
                $y = 14 + ($bytes[$index + 23] % 178)
                $size = 2 + ($bytes[$index + 26] % 4)
                $graphics.FillEllipse($sparkleBrush, $x, $y, $size, $size)
            }
        }
        finally { $sparkleBrush.Dispose() }

        $bitmap.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png)
    }
    finally {
        $graphics.Dispose()
        $bitmap.Dispose()
    }
}

$resolvedCatalogPath = [System.IO.Path]::GetFullPath($CatalogPath)
$resolvedOutputDirectory = [System.IO.Path]::GetFullPath($OutputDirectory)
if (-not (Test-Path -LiteralPath $resolvedCatalogPath -PathType Leaf)) {
    throw "Gallery component catalog was not found at '$resolvedCatalogPath'."
}

$outputRoot = [System.IO.Path]::GetPathRoot($resolvedOutputDirectory)
if ([string]::Equals(
        $resolvedOutputDirectory.TrimEnd([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar),
        $outputRoot.TrimEnd([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar),
        [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "Refusing to generate card backgrounds in filesystem root '$resolvedOutputDirectory'."
}

$catalog = Get-Content -Raw -LiteralPath $resolvedCatalogPath
$pattern = 'Item<[^>]+>\(\s*"(?<tag>[^"]+)"\s*,\s*"(?<title>[^"]+)"\s*,\s*"(?<category>[^"]+)"'
$matches = [System.Text.RegularExpressions.Regex]::Matches(
    $catalog,
    $pattern,
    [System.Text.RegularExpressions.RegexOptions]::Singleline)

if ($matches.Count -ne 114) {
    throw "Expected 114 Gallery components, but parsed $($matches.Count)."
}

$items = @($matches | ForEach-Object {
    [pscustomobject]@{
        PageTag = $_.Groups['tag'].Value
        Title = $_.Groups['title'].Value
        Category = $_.Groups['category'].Value
    }
})

$duplicateTags = @($items | Group-Object PageTag | Where-Object Count -ne 1)
if ($duplicateTags.Count -ne 0) {
    throw "Gallery page tags must be unique: $($duplicateTags.Name -join ', ')."
}

$invalidTags = @($items | Where-Object { $_.PageTag -notmatch '^[a-z0-9]+$' })
if ($invalidTags.Count -ne 0) {
    throw "Gallery page tags must contain only lowercase ASCII letters and digits: $($invalidTags.PageTag -join ', ')."
}

if (-not $PSCmdlet.ShouldProcess(
        $resolvedOutputDirectory,
        'Replace generated SaaS card background PNG files')) {
    return
}

[System.IO.Directory]::CreateDirectory($resolvedOutputDirectory) | Out-Null
Get-ChildItem -LiteralPath $resolvedOutputDirectory -File -Filter '*.png' -ErrorAction SilentlyContinue |
    Remove-Item -Force

foreach ($item in $items) {
    $outputPath = Join-Path $resolvedOutputDirectory "$($item.PageTag).png"
    New-SaasBackground -PageTag $item.PageTag -Category $item.Category -Path $outputPath
}

$generated = @(Get-ChildItem -LiteralPath $resolvedOutputDirectory -File -Filter '*.png')
$uniqueHashes = @($generated | Get-FileHash -Algorithm SHA256 | Select-Object -ExpandProperty Hash -Unique)
if ($generated.Count -ne 114 -or $uniqueHashes.Count -ne 114) {
    throw "SaaS background generation failed: files=$($generated.Count), unique=$($uniqueHashes.Count)."
}

Write-Host "Generated $($generated.Count) unique SaaS card backgrounds in '$resolvedOutputDirectory'."
