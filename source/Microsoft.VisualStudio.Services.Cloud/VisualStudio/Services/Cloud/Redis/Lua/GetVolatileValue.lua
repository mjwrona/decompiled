-- KEYS[i] - Redis key to fetch value for
-- ARGV[1] - Maximum message size to return (but we would still return at least 1 value)

local maxSize = tonumber(ARGV[1])
-- unpack has limits (up to 8000 items) or it would stack overflow
-- capping at 5000 (voluntarily), client should expect reponse of arbitrary size anyway
local values = redis.call('MGET',unpack(KEYS,1,math.min(#KEYS,5000)))

local totalSize = 0
for i=1,#KEYS do
  if values[i] then
    totalSize = totalSize + string.len(values[i])
  end
  -- we would always return at least the first element (no matter what is the size)
  if totalSize > maxSize and i > 1 then
    -- this effectively truncates the array when we marshal it to Redis
    values[i] = nil
    break
  end
end

return values
