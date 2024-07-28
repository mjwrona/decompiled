-- KEYS[i] - Redis key to be incremented
-- ARGV[1] - Window duration setting

local keys = #KEYS
local duration = tonumber(ARGV[1])

if duration == 0 then
  -- explicitly return error because dividing by zero won't throw and instead would produce math.huge (#INF)
  return redis.error_reply("Unexpected zero window duration")
end

local result = {}
local values = redis.call('MGET',unpack(KEYS))

for i=1,keys do
  local value = 0
  if values[i] then
    local ttl = redis.call('PTTL',KEYS[i])
    value = tonumber(values[i])

    -- determine how much we should replenish for this key
    local passed = duration - ttl
    local replenishment = passed / duration * value

    -- replenish before returning; this does NOT actually update the Redis cache
    value = math.max(value - replenishment, 0)
  end

  -- round up the current value before returning it as a result
  result[i] = math.ceil(value)
end

return result
