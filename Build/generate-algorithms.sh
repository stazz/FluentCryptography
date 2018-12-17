#!/bin/sh

SCRIPTPATH=$(readlink -f "$0")
SCRIPTDIR=$(dirname "$SCRIPTPATH")
GIT_ROOT=$(readlink -f "${SCRIPTDIR}/..")

# Generate algorithms using Jinja2 Docker image
docker run \
  --rm \
  -v "${GIT_ROOT}/Source/Code/FluentCryptography.Digest/Algorithms/:/work-dir/algorithms/:rw" \
  -v "${GIT_ROOT}/Build/FluentCryptography.SHADigestAlgorithmGenerator/:/work-dir/build/code/:ro" \
  -v "${GIT_ROOT}/Source/Directory.Build.BuildTargetFolders.props:/work-dir/build/Directory.Build.props:ro" \
  "microsoft/dotnet:${DOTNET_VERSION}-sdk-alpine" \
  dotnet run \
  -c Release \
  -p /work-dir/build/code \
  --no-launch-profile \
  -- \
  /work-dir/algorithms