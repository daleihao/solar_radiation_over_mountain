library(elevatr)
library(horizon)


# load DEM data
data(lake)
elevation <- get_elev_raster(lake, z = 9)
plot(elevation)
plot(lake, add = TRUE)

# slope
slopes = terrain(elevation, opt="slope", unit="degrees", neighbors=8)
plot(slopes)

# slope
aspects = terrain(elevation, opt="aspect", unit="degrees", neighbors=8)
plot(aspects)

# sky view factor
skyviewfactors <- svf(elevation, nAngles=8, maxDist=2000, ll=FALSE)
plot(skyviewfactors)

# horizon angles
horizons <- horizonSearch(elevation, 60, maxDist = 2000, degrees = TRUE,
              ll = FALSE)
plot(horizons)
